using System;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IRetryService<T>
    {
        T2 Retry<T1, T2>(Func<T1, T2> method, T1 argument);
    }
}
