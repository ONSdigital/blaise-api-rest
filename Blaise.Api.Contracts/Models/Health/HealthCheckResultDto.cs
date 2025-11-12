namespace Blaise.Api.Contracts.Models.Health
{
    using Blaise.Api.Contracts.Enums;
    using Newtonsoft.Json;

    public class HealthCheckResultDto
    {
        public HealthCheckResultDto(HealthCheckType checkType, HealthStatusType statusType)
        {
            StatusType = statusType;
            CheckType = checkType;
        }

        [JsonProperty(PropertyName = "Health check type")]
        public HealthCheckType CheckType { get; }

        [JsonProperty(PropertyName = "status")]
        public HealthStatusType StatusType { get; }
    }
}
