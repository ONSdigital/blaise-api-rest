namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Health;

    public interface IHealthCheckService
    {
        IEnumerable<HealthCheckResultDto> PerformCheck();
    }
}
