using System.Collections.Generic;
using Blaise.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces
{
    public interface IServerParkDtoMapper
    {
        IEnumerable<ServerParkDto> MapToDto(IEnumerable<IServerPark> serverParks);
        ServerParkDto MapToDto(IServerPark serverPark);
    }
}