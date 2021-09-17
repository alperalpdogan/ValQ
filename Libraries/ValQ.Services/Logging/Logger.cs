using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ValQ.Core;
using ValQ.Core.Domain.Logging;
using ValQ.Data.Repository;

namespace ValQ.Services.Logging
{
    public class Logger : ILogger
    {
        private readonly IRepository<Log> _logRepository;

        public async Task DeleteLogAsync(Log log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            await _logRepository.DeleteAsync(log, false);
        }

        public async Task DeleteLogsAsync(IList<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            await _logRepository.DeleteAsync(logs, false);
        }

        public async Task ErrorAsync(string message, Exception exception = null)
        {
            var log = new Log
            {
                LogLevel = Core.Domain.Logging.LogLevel.Error,
                ShortMessage = message,
                FullMessage = exception?.ToString() ?? string.Empty,
                IpAddress = "1.1.1.1",
                CreatedAt = DateTime.Now
            };

            await _logRepository.InsertAsync(log, false);
        }

        public async Task<IPagedList<Log>> GetAllLogsAsync(DateTime? from = null, DateTime? to = null, string message = "", LogLevel? logLevel = null, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            var logs = await _logRepository.GetAllPagedAsync(query =>
            {
                if (from.HasValue)
                    query = query.Where(l => from.Value <= l.CreatedAt);
                if (to.HasValue)
                    query = query.Where(l => to.Value >= l.CreatedAt);
                if (logLevel.HasValue)
                {
                    query = query.Where(l => l.LogLevel == logLevel);
                }

                if (!string.IsNullOrEmpty(message))
                    query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
                query = query.OrderByDescending(l => l.CreatedAt);

                return query;
            }, pageNumber, pageSize);

            return logs;
        }

        public async Task<Log> GetLogByIdAsync(int logId)
        {
            return await _logRepository.GetByIdAsync(logId);
        }

        public async Task<IList<Log>> GetLogByIdsAsync(int[] logIds)
{
            return await _logRepository.GetByIdsAsync(logIds);
        }

        public async Task InformationAsync(string message, Exception exception = null)
        {
            await _logRepository.InsertAsync(new Log()
            {
                LogLevel = LogLevel.Information,
                CreatedAt = DateTime.Now,
                IpAddress = "1111",
                FullMessage = exception != null ? exception.ToString() : "",
                ShortMessage = message
            });
        }

        public async Task InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "")
        {
            await _logRepository.InsertAsync(new Log()
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                CreatedAt = DateTime.Now,
                IpAddress = "111"
            });
        }

        public async Task WarningAsync(string message, Exception exception = null)
        {
            await _logRepository.InsertAsync(new Log()
            {
                CreatedAt = DateTime.Now,
                IpAddress = "111",
                LogLevel = LogLevel.Warning,
                ShortMessage = message,
                FullMessage = exception != null ? exception.ToString() : "",
            });
        }
    }
}
