using System.Collections.Generic;
using Blaise.Api.Contracts.Models.ServerPark;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IServerParkService
    {
        IEnumerable<ServerParkDto> GetServerParks();

        ServerParkDto GetServerPark(string serverParkName);

        bool ServerParkExists(string serverParkName);
    }
}