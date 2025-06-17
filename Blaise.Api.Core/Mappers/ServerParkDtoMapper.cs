using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class ServerParkDtoMapper : IServerParkDtoMapper
    {
        private readonly IQuestionnaireDtoMapper _mapper;

        public ServerParkDtoMapper(IQuestionnaireDtoMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<ServerParkDto> MapToServerParkDtos(IEnumerable<IServerPark> serverParks)
        {
            var serverParkDtoList = new List<ServerParkDto>();

            foreach (var serverPark in serverParks)
            {
                serverParkDtoList.Add(MapToServerParkDto(serverPark));
            }

            return serverParkDtoList;
        }

        public ServerParkDto MapToServerParkDto(IServerPark serverPark)
        {
            return new ServerParkDto
            {
                Name = serverPark.Name,
                LoadBalancer = serverPark.LoadBalancer,
                Questionnaires = _mapper.MapToQuestionnaireDtos(serverPark.Surveys),
                Servers = MapToServerDtos(serverPark.Servers)
            };
        }

        private static IEnumerable<ServerDto> MapToServerDtos(IServerCollection servers)
        {
            var serverDtoList = new List<ServerDto>();

            foreach (var server in servers)
            {
                serverDtoList.Add(new ServerDto
                {
                    Name = server.Name,
                    BlaiseVersion = ((IServer2)server).BlaiseVersion.ToString(),
                    LogicalServerName = server.LogicalRoot,
                    Roles = server.Roles.ToList()
                });
            }

            return serverDtoList;
        }
    }
}
