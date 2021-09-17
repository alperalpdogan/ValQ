using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core.Domain.Logging;
using ValQ.Core;

namespace ValQ.Services.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteLogAsync(Log log);

        /// <summary>
        /// Deletes a log items
        /// </summary>
        /// <param name="logs">Log items</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteLogsAsync(IList<Log> logs);

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log item items
        /// </returns>
        Task<IPagedList<Log>> GetAllLogsAsync(DateTime? from = null, DateTime? to = null,
            string message = "", LogLevel? logLevel = null,
            int pageNumber = 1, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log item
        /// </returns>
        Task<Log> GetLogByIdAsync(int logId);

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log items
        /// </returns>
        Task<IList<Log>> GetLogByIdsAsync(int[] logIds);

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a log item
        /// </returns>
        Task InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "");

        /// <summary>
        /// Information
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InformationAsync(string message, Exception exception = null);

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task WarningAsync(string message, Exception exception = null);

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ErrorAsync(string message, Exception exception = null);
    }
}
