using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Mappers
{
    public class OpenConnectionModelMapper : IOpenConnectionModelMapper
    {
        public IEnumerable<OpenConnectionDto> MapTOpenConnectionDtos(IEnumerable<OpenConnectionModel> openConnectionModels)
        {
            var openConnectionDtoList = new List<OpenConnectionDto>();

            if (openConnectionModels == null)
            {
                return openConnectionDtoList;
            }
  
            foreach (var openConnectionModel in openConnectionModels)
            {
                openConnectionDtoList.Add(new OpenConnectionDto
                {
                    ConnectionType = openConnectionModel.ConnectionType,
                    Connections = openConnectionModel.Connections
                });
            }

            return openConnectionDtoList;
        }
    }
}