using Blaise.Api.Contracts.Enums;
using Newtonsoft.Json;

namespace Blaise.Api.Contracts.Models.Health
{
    public class HealthCheckResultDto
    {
        [JsonProperty(PropertyName = "Health check type")]
        public readonly HealthCheckType CheckType;

        [JsonProperty(PropertyName = "status")]
        public readonly HealthStatusType StatusType;

        public HealthCheckResultDto(HealthCheckType checkType, HealthStatusType statusType)
        {
            StatusType = statusType;
            CheckType = checkType;
        }
    }
}
