using System;
using Blaise.Api.Core.Interfaces.Services;
using Polly;
using Polly.Retry;

namespace Blaise.Api.Core.Services
{
    public class RetryService<T> : IRetryService<T>
        where T : Exception
    {
        private const int MaxRetryAttempts = 3;
        private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(1);
        private readonly RetryPolicy _retryPolicy;

        public RetryService()
        {
            _retryPolicy = Policy
                .Handle<T>()
                .WaitAndRetry(MaxRetryAttempts, i => _pauseBetweenFailures);
        }

        public T2 Retry<T1, T2>(Func<T1, T2> method, T1 argument)
        {
            return _retryPolicy.Execute(() => method(argument));
        }
    }
}
