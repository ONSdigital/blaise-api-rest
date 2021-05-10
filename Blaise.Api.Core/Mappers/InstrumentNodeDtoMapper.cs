using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class InstrumentNodeDtoMapper : IInstrumentNodeDtoMapper
    {
        public IEnumerable<InstrumentNodeDto> MapToInstrumentNodeDtos(IMachineConfigurationCollection instrumentConfigurations)
        {
            var instrumentNodes = new List<InstrumentNodeDto>();

            if (instrumentConfigurations == null)
            {
                return instrumentNodes;
            }

            foreach (var configuration in instrumentConfigurations)
            {
                instrumentNodes.Add(new InstrumentNodeDto
                {
                    NodeName = configuration.Key,
                    NodeStatus = configuration.Value.Status
                });
            }

            return instrumentNodes;
        }
    }
}
