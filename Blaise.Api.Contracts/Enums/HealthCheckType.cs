namespace Blaise.Api.Contracts.Enums
{
    using System.Runtime.Serialization;

    public enum HealthCheckType
    {
        [EnumMember(Value = "Connection model")]
        ConnectionModel,

        [EnumMember(Value = "Blaise connection")]
        Connection,

        [EnumMember(Value = "Remote data server connection")]
        RemoteDataServer,

        [EnumMember(Value = "Remote Cati management connection")]
        RemoteCatiManagement,
    }
}
