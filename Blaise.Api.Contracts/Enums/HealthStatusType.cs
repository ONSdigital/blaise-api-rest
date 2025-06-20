using System.Runtime.Serialization;

namespace Blaise.Api.Contracts.Enums
{
    public enum HealthStatusType
    {
        [EnumMember(Value = "OK")]
        Ok,

        [EnumMember(Value = "ERROR")]
        Error
    }
}