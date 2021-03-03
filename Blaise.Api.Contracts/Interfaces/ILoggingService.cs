using System;

namespace Blaise.Api.Contracts.Interfaces
{
    public interface ILoggingService
    {
        void LogInfo(string message);

        void LogWarn(string message);

        void LogError(string message, Exception exception);
    }
}