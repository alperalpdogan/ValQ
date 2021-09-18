using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Localization;

namespace ValQ.Services.Localization
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteLanguageAsync(Language language);

        /// <summary>
        /// Gets language by specified language culture
        /// </summary>
        /// <param name="langCulture">language culture </param>
        /// <returns></returns>
        Task<Language> GetLanguageByCultureAsync(string langCulture);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the languages
        /// </returns>
        Task<IList<Language>> GetAllLanguagesAsync(bool showHidden = false, int storeId = 0);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language
        /// </returns>
        Task<Language> GetLanguageByIdAsync(int languageId);

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertLanguageAsync(Language language);

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateLanguageAsync(Language language);
    }
}
