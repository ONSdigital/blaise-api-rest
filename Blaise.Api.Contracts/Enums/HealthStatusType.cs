namespace Blaise.Api.Contracts.Enums
{
    using System.Runtime.Serialization;

    public enum HealthStatusType
    {
        [EnumMember(Value = "OK")]
        Ok,

        [EnumMember(Value = "ERROR")]
        Error,
    }
}
