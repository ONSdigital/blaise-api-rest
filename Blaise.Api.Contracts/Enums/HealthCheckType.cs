using System.Runtime.Serialization;

namespace Blaise.Api.Contracts.Enums
{
    public enum HealthCheckType
    {
        [EnumMember(Value = "Connection model")]
        ConnectionModel,

        [EnumMember(Value = "Blaise connection")]
        Connection,

        [EnumMember(Value = "Remote data server connection")]
        RemoteDataServer,

        [EnumMember(Value = "Remote Cati management connection")]
        RemoteCatiManagement
    }
}