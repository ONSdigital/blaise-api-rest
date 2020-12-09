using System.Collections.Generic;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class ServerParkService : IServerParkService
    {
        private readonly IBlaiseServerParkApi _blaiseApi;
        private readonly IServerParkDtoMapper _mapper;

        public ServerParkService(
            IBlaiseServerParkApi blaiseApi,
            IServerParkDtoMapper mapper)
        {
            _blaiseApi = blaiseApi;
            _mapper = mapper;
        }

        public IEnumerable<string> GetServerParkNames()
        {
            return _blaiseApi.GetNamesOfServerParks();
        }

        public IEnumerable<ServerParkDto> GetServerParks()
        {
            var serverParks = _blaiseApi.GetServerParks();

            return _mapper.MapToDto(serverParks);
        }

        public ServerParkDto GetServerPark(string serverParkName)
        {
            var serverPark = _blaiseApi.GetServerPark(serverParkName);

            return _mapper.MapToDto(serverPark);
        }

        public bool ServerParkExists(string serverParkName)
        {
            return _blaiseApi.ServerParkExists(serverParkName);
        }
    }
}
