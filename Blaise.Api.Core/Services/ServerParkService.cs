using System.Collections.Generic;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class ServerParkService : IServerParkService
    {
        private readonly IBlaiseApi _blaiseApi;
        private readonly IServerParkDtoMapper _mapper;

        public ServerParkService(
            IBlaiseApi blaiseApi,
            IServerParkDtoMapper mapper)
        {
            _blaiseApi = blaiseApi;
            _mapper = mapper;
        }

        public IEnumerable<string> GetServerParkNames()
        {
            return _blaiseApi.GetServerParkNames(_blaiseApi.GetDefaultConnectionModel());
        }

        public IEnumerable<ServerParkDto> GetServerParks()
        {
            var serverParks = _blaiseApi.GetServerParks(_blaiseApi.GetDefaultConnectionModel());

            return _mapper.MapToDto(serverParks);
        }

        public ServerParkDto GetServerPark(string serverParkName)
        {
            var serverPark = _blaiseApi.GetServerPark(
                _blaiseApi.GetDefaultConnectionModel(), 
                serverParkName);

            return _mapper.MapToDto(serverPark);
        }

        public bool ServerParkExists(string serverParkName)
        {
            return _blaiseApi.ServerParkExists(
                    _blaiseApi.GetDefaultConnectionModel(),
                    serverParkName);
        }
    }
}
