﻿using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class ServerParkDtoMapper : IServerParkDtoMapper
    {
        private readonly IInstrumentDtoMapper _mapper;

        public ServerParkDtoMapper(IInstrumentDtoMapper mapper)
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
                Instruments = _mapper.MapToInstrumentDtos(serverPark.Surveys),
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
                    LogicalServerName = server.LogicalRoot,
                    Roles = server.Roles.ToList()
                });
            }

            return serverDtoList;
        }
    }
}
