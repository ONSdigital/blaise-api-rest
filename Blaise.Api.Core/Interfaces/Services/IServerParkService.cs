namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.ServerPark;

    public interface IServerParkService
    {
        IEnumerable<ServerParkDto> GetServerParks();

        ServerParkDto GetServerPark(string serverParkName);

        bool ServerParkExists(string serverParkName);
    }
}
