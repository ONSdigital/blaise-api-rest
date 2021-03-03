using Blaise.Api.Contracts.Interfaces;
using System;
using System.Diagnostics;

namespace Blaise.Api.Logging.Services
{
    public class EventLogging : ILoggingService
    {
        public void LogError(string message, Exception exception)
        {
            EventLog.WriteEntry("Rest API", message, EventLogEntryType.Error, 0,0, Convert.ToBase64CharArray(exception.InnerException));
        }

        public void LogInfo(string message)
        {
            EventLog.WriteEntry("Rest API", message, EventLogEntryType.Information);
        }
    }
}
