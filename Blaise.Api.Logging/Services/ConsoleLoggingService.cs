using System;
using Blaise.Api.Contracts.Interfaces;

namespace Blaise.Api.Logging.Services
{
    public class ConsoleLoggingService : ILoggingService
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
            Console.WriteLine($"Error - {message}: {exception.Message}, {exception.InnerException}");
        }
    }
}
