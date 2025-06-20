using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Health;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IHealthCheckService
    {
        IEnumerable<HealthCheckResultDto> PerformCheck();
    }
}