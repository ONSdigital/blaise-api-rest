﻿using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
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

        public IEnumerable<ServerParkDto> GetServerParks()
        {
            var serverParks = _blaiseApi.GetServerParks();

            return _mapper.MapToServerParkDtos(serverParks);
        }

        public ServerParkDto GetServerPark(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var serverPark = _blaiseApi.GetServerPark(serverParkName);

            return _mapper.MapToServerParkDto(serverPark);
        }

        public bool ServerParkExists(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseApi.ServerParkExists(serverParkName);
        }

        public void RegisterMachineOnServerPark(string serverParkName, MachineDto machineDto)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            machineDto.MachineName.ThrowExceptionIfNullOrEmpty("machineDto.MachineName");
            machineDto.LogicalRootName.ThrowExceptionIfNullOrEmpty("machineDto.LogicalRootName");
            machineDto.Roles.ThrowExceptionIfNullOrEmpty("machineDto.Roles");

            _blaiseApi.RegisterMachineOnServerPark(serverParkName, machineDto.MachineName,
                machineDto.LogicalRootName, machineDto.Roles);
        }
    }
}
