namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.ServerPark;
    using StatNeth.Blaise.API.ServerManager;

    public interface IServerParkDtoMapper
    {
        IEnumerable<ServerParkDto> MapToServerParkDtos(IEnumerable<IServerPark> serverParks);

        ServerParkDto MapToServerParkDto(IServerPark serverPark);
    }
}
