using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ValQ.Core.Caching;
using ValQ.Core.Domain.Common;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Util;
using ValQ.Core;
using ValQ.Data.Repository;
using ValQ.Services.Localization.Caching;
using ValQ.Core.Services.Localization;
using Microsoft.EntityFrameworkCore;

namespace ValQ.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILanguageService _languageService;
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IWorkContext _workContext;

        public LocalizationService(ILanguageService languageService,
                                   IRepository<LocaleStringResource> lsrRepository,
                                   IStaticCacheManager staticCacheManager,
                                   ILocalizedEntityService localizedEntityService,
                                   IWorkContext workContext)
        {
            _languageService = languageService;
            _lsrRepository = lsrRepository;
            _staticCacheManager = staticCacheManager;
            _localizedEntityService = localizedEntityService;
            _workContext = workContext;
        }

        #region Utilities

        protected virtual HashSet<(string name, string value)> LoadLocaleResourcesFromStream(StreamReader xmlStreamReader, string language)
        {
            var result = new HashSet<(string name, string value)>();

            using (var xmlReader = XmlReader.Create(xmlStreamReader))
                while (xmlReader.ReadToFollowing("Language"))
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    using var languageReader = xmlReader.ReadSubtree();
                    while (languageReader.ReadToFollowing("LocaleResource"))
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is string name)
                        {
                            using var lrReader = languageReader.ReadSubtree();
                            if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                                result.Add((name.ToLowerInvariant(), lrReader.ReadString()));
                        }

                    break;
                }

            return result;
        }
        #endregion

        public async Task AddLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null)
        {
            //first delete all previous locales with the passed names if they exist
            await DeleteLocaleResourcesAsync(resources.Keys.ToList(), languageId);

            //insert new locale resources
            var locales = (await _languageService.GetAllLanguagesAsync(true))
                .Where(language => !languageId.HasValue || language.Id == languageId.Value)
                .SelectMany(language => resources.Select(resource => new LocaleStringResource
                {
                    LanguageId = language.Id,
                    ResourceName = resource.Key,
                    ResourceValue = resource.Value
                }))
                .ToList();

            await _lsrRepository.InsertAsync(locales, false);

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        public async Task AddOrUpdateLocaleResourceAsync(string resourceName, string resourceValue, string languageCulture = null)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                if (!string.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.LanguageIETFCode))
                    continue;

                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await UpdateLocaleStringResourceAsync(lsr);
                }
            }
        }

        public async Task DeleteLocaleResourceAsync(string resourceName)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id);
                if (lsr != null)
                    await DeleteLocaleStringResourceAsync(lsr);
            }
        }

        public async Task DeleteLocaleResourcesAsync(IList<string> resourceNames, int? languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        public async Task DeleteLocaleResourcesAsync(string resourceNamePrefix, int? languageId = null)
        {
            await _lsrRepository.DeleteAsync(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                !string.IsNullOrEmpty(locale.ResourceName) &&
                locale.ResourceName.StartsWith(resourceNamePrefix, StringComparison.InvariantCultureIgnoreCase));

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        public async Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.DeleteAsync(localeStringResource);
        }

        public async Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId)
        {
            return await _lsrRepository.GetByIdAsync(localeStringResourceId, cache => default);
        }

        public async Task<LocaleStringResource> GetLocaleStringResourceByNameAsync(string resourceName, int languageId)
        {
            var query = from lsr in _lsrRepository.Table
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                        select lsr;

            var localeStringResource = await query.FirstOrDefaultAsync();

            return localeStringResource;
        }

        public async Task<TPropType> GetLocalizedAsync<TEntity, TPropType>(TEntity entity, Expression<Func<TEntity, TPropType>> keySelector, int? languageId = null, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (!languageId.HasValue)
                languageId = workingLanguage.Id;

            if (languageId > 0)
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    resultStr = await _localizedEntityService
                        .GetLocalizedValueAsync(languageId.Value, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (!string.IsNullOrEmpty(resultStr) || !returnDefaultValue)
                return result;
            var localizer = keySelector.Compile();
            result = localizer(entity);

            return result;
        }

        public async Task<string> GetResourceAsync(string resourceKey)
        {
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (workingLanguage != null)
                return await GetResourceAsync(resourceKey, workingLanguage.Id);

            return string.Empty;

        }

        public async Task<string> GetResourceAsync(string resourceKey, int languageId, bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();

            var key = _staticCacheManager.PrepareKeyForDefaultCache(LocalizationDefaults.LocaleStringResourcesByNameCacheKey
                , languageId, resourceKey);

            var query = from l in _lsrRepository.Table
                        where l.ResourceName == resourceKey
                                && l.LanguageId == languageId
                        select l.ResourceValue;

            var lsr = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

            if (lsr != null)
                result = lsr;


            if (!string.IsNullOrEmpty(result))
                return result;

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        public async Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (xmlStreamReader.EndOfStream)
                return;

            var lsNamesList = new Dictionary<string, LocaleStringResource>();

            foreach (var localeStringResource in _lsrRepository.Table.Where(lsr => lsr.LanguageId == language.Id)
                .OrderBy(lsr => lsr.Id))
                lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

            var lrsToUpdateList = new List<LocaleStringResource>();
            var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in LoadLocaleResourcesFromStream(xmlStreamReader, language.Name))
            {
                if (lsNamesList.ContainsKey(name))
                {
                    if (!updateExistingResources)
                        continue;

                    var lsr = lsNamesList[name];
                    lsr.ResourceValue = value;
                    lrsToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { LanguageId = language.Id, ResourceName = name, ResourceValue = value };
                    lrsToInsertList[name] = lsr;
                }
            }

            await _lsrRepository.UpdateAsync(lrsToUpdateList, false);
            await _lsrRepository.InsertAsync(lrsToInsertList.Values.ToList(), false);

            //clear cache
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<LocaleStringResource>.Prefix);
        }

        public async Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.InsertAsync(localeStringResource);
        }

        public async Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.UpdateAsync(localeStringResource);
        }

        public async Task<string> GetLocalizedEnumAsync<TEnum>(TEnum enumValue, int? languageId = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            //localized value
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var resourceName = $"{LocalizationDefaults.EnumLocaleStringResourcesPrefix}{typeof(TEnum)}.{enumValue}";
            var result = await GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, false, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = enumValue.ToString().Humanize(LetterCasing.Title);

            return result;
        }
    }
}
