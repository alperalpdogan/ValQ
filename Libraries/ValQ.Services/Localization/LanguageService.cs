using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Caching;
using ValQ.Core.Domain.Localization;
using ValQ.Data.Repository;
using ValQ.Services.Localization.Caching;

namespace ValQ.Core.Services.Localization
{
    public class LanguageService : ILanguageService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public LanguageService(IRepository<Language> languageRepository, IStaticCacheManager staticCacheManager)
        {
            _languageRepository = languageRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        public async Task DeleteLanguageAsync(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            await _languageRepository.DeleteAsync(language);
        }

        public async Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, int storeId = 0)
        {
            //cacheable copy
            var key = _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Language>.AllCacheKey);

            var languages = await _staticCacheManager.GetAsync(key, async () =>
            {
                var allLanguages = await _languageRepository.GetAllAsync(query =>
                {
                    return query;
                });

                return allLanguages;
            });

            return languages;
        }

        public async Task<Language> GetLanguageByCultureAsync(string langCulture)
        {
            if (string.IsNullOrWhiteSpace(langCulture))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(LocalizationDefaults.LanguageByCultureCacheKey, langCulture);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                return await _languageRepository.Table.Where(o => o.LanguageIETFCode == langCulture).SingleOrDefaultAsync();
            });
        }

        public async Task<Language> GetLanguageByIdAsync(int languageId)
        {
            return await _languageRepository.GetByIdAsync(languageId, cache => _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<Language>.AllCacheKey));
        }

        public async Task InsertLanguageAsync(Language language)
        {
            await _languageRepository.InsertAsync(language);
        }

        public async Task UpdateLanguageAsync(Language language)
        {
            await _languageRepository.UpdateAsync(language);
        }

        #endregion
    }
}
