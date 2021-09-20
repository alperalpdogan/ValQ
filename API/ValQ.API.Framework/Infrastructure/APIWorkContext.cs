using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;
using ValQ.Core.Domain.Users;
using ValQ.Core;
using ValQ.Services.Users;
using ValQ.Services.Localization;

namespace ValQ.API.Framework.Infrastructure
{
    public class APIWorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILanguageService _languageService;
        private readonly IUserService _userService;

        public APIWorkContext(IHttpContextAccessor httpContextAccessor,
                              ILanguageService languageService,
                              IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _languageService = languageService;
            _userService = userService;
        }

        public async Task<Language> GetWorkingLanguageAsync()
        {
            if (_currentLanguage != null)
                return _currentLanguage;

            var userLangs = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"].ToString();
            var firstLang = userLangs.Split(',').FirstOrDefault();

            var language = await _languageService.GetLanguageByCultureAsync(firstLang);

            if (language == null)
                language = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault();

            _currentLanguage = language;

            return _currentLanguage;
        }

        public Task SetWorkingLanguageAsync(Language language)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var userId = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;

            _cachedUser = await _userService.GetUserById(userId);

            return _cachedUser;
        }

        #region Cached properties
        private User _cachedUser;
        private Language _currentLanguage;
        #endregion

    }
}
