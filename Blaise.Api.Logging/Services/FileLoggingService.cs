using System;
using Blaise.Api.Contracts.Interfaces;
using log4net;

namespace Blaise.Api.Logging.Services
{
    public class FileLoggingService : ILoggingService
    {
        private readonly ILog _fileLogger;

        public FileLoggingService()
        {
            _fileLogger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void LogInfo(string message)
        {
            _fileLogger.Info(message);
        }

        public void LogError(string message, Exception exception)
        {
            _fileLogger.Error($"{message}: {exception.Message}, {exception.InnerException}");
        }
    }
}
