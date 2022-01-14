using Blaise.Api.Contracts.Interfaces;
using System;

namespace Blaise.Api.Logging.Services
{
    public class ConsoleLogging : ILoggingService
    {
        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarn(string message)
        {
            Console.WriteLine($"Warning - {message}");
        }

        public void LogError(string message, Exception exception)
        {
            Console.WriteLine($"Error - {message}: {exception}");
        }
    }
}
