using Blaise.Api.Contracts.Interfaces;
using System;
using System.Diagnostics;

namespace Blaise.Api.Logging.Services
{
    public class TestEventLogging : ILoggingService
    {
        public void LogError(string message, Exception exception)
        {
            EventLog.WriteEntry("Rest API TESTS", $"RESTAPI TESTS: {message}: {exception}", EventLogEntryType.Error);
        }

        public void LogInfo(string message)
        {
            EventLog.WriteEntry("Rest API TESTS", $"RESTAPI TESTS: {message}", EventLogEntryType.Information);
        }

        public void LogWarn(string message)
        {
            EventLog.WriteEntry("Rest API TESTS", $"RESTAPI TESTS: {message}", EventLogEntryType.Warning);
        }
    }
}
