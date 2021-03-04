using Blaise.Api.Contracts.Interfaces;
using System;
using System.Diagnostics;
using System.Text;

namespace Blaise.Api.Logging.Services
{
    public class EventLogging : ILoggingService
    {
        public void LogError(string message, Exception exception)
        {
            EventLog.WriteEntry("Rest API", $"RESTAPI: {message}: {exception.Message}, {exception.InnerException}", EventLogEntryType.Error);
        }

        public void LogInfo(string message)
        {
            EventLog.WriteEntry("Rest API", $"RESTAPI: {message}", EventLogEntryType.Information);
        }

        public void LogWarn(string message)
        {
            EventLog.WriteEntry("Rest API", $"RESTAPI: {message}", EventLogEntryType.Warning);
        }
    }
}
