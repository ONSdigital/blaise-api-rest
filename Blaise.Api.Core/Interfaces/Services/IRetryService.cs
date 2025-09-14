namespace Blaise.Api.Core.Interfaces.Services
{
    using System;

    public interface IRetryService<T>
    {
        T2 Retry<T1, T2>(Func<T1, T2> method, T1 argument);
    }
}
