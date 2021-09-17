using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Caching;
using ValQ.Core.Domain.Localization;

namespace ValQ.Services.Localization.Caching
{
    public static class LocalizationDefaults
    {
        #region Prefixes

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "Enums.";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static CacheKey LocalizedPropertyCacheKey => new CacheKey("ValQ.localizedproperty.value.{0}-{1}-{2}-{3}");

        #endregion

        #region Resource
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static CacheKey LocaleStringResourcesByNameCacheKey => new CacheKey("ValQ.localestringresource.byname.{0}-{1}", LocaleStringResourcesByNamePrefix, EntityCacheDefaults<LocaleStringResource>.Prefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesByNamePrefix => "ValQ.localestringresource.byname.{0}";
        #endregion

        #region Language
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : culture
        /// </remarks>
        public static CacheKey LanguageByCultureCacheKey => new CacheKey("ValQ.language.byculture.{0}");

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : culture
        /// </remarks>
        public static string LanguageByCulturePrefix => "ValQ.language.byculture.{0}";

        #endregion

    }
}
