using System.Collections.Generic;
using Blaise.Api.Contracts.Models;

namespace Blaise.Api.Core.Interfaces
{
    public interface IServerParkService
    {
        IEnumerable<string> GetServerParkNames();

        IEnumerable<ServerParkDto> GetServerParks();

        ServerParkDto GetServerPark(string serverParkName);

        bool ServerParkExists(string serverParkName);
    }
}