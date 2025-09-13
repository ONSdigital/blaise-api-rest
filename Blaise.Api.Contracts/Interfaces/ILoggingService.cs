namespace Blaise.Api.Contracts.Interfaces
{
    using System;

    public interface ILoggingService
    {
        void LogInfo(string message);

        void LogWarn(string message);

        void LogError(string message, Exception exception);
    }
}
