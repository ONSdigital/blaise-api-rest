using System.Collections.Generic;
using Blaise.Api.Contracts.Enums;
using Blaise.Api.Contracts.Models.Health;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IBlaiseHealthApi _blaiseHealthApi;

        public HealthCheckService(IBlaiseHealthApi blaiseHealthApi)
        {
            _blaiseHealthApi = blaiseHealthApi;
        }

        public IEnumerable<HealthCheckResultDto> PerformCheck()
        {
            return new List<HealthCheckResultDto>
            {
                CheckConnectionModel(),
                CheckConnection(),
                CheckRemoteDataServerConnection(),
                CheckRemoteCatiManagementConnection()
            };
        }

        private HealthCheckResultDto CheckConnectionModel()
        {
            return _blaiseHealthApi.ConnectionModelIsHealthy() 
                ? new HealthCheckResultDto(HealthCheckType.ConnectionModel, HealthStatusType.Ok) 
                : new HealthCheckResultDto(HealthCheckType.ConnectionModel, HealthStatusType.Error);
        }

        private HealthCheckResultDto CheckConnection()
        {
            return _blaiseHealthApi.ConnectionToBlaiseIsHealthy()
                ? new HealthCheckResultDto(HealthCheckType.Connection, HealthStatusType.Ok)
                : new HealthCheckResultDto(HealthCheckType.Connection, HealthStatusType.Error);
        }

        private HealthCheckResultDto CheckRemoteDataServerConnection()
        {
            return _blaiseHealthApi.RemoteConnectionToBlaiseIsHealthy()
                ? new HealthCheckResultDto(HealthCheckType.RemoteDataServer, HealthStatusType.Ok)
                : new HealthCheckResultDto(HealthCheckType.RemoteDataServer, HealthStatusType.Error);
        }

        private HealthCheckResultDto CheckRemoteCatiManagementConnection()
        {
            return _blaiseHealthApi.RemoteConnectionToCatiIsHealthy()
                ? new HealthCheckResultDto(HealthCheckType.RemoteCatiManagement, HealthStatusType.Ok)
                : new HealthCheckResultDto(HealthCheckType.RemoteCatiManagement, HealthStatusType.Error);
        }
    }
}
