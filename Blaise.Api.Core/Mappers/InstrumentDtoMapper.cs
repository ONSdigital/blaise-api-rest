using System.Collections.Generic;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class InstrumentDtoMapper : IInstrumentDtoMapper
    {
        public IEnumerable<InstrumentDto> MapToDto(IEnumerable<ISurvey> instruments)
        {
            var instrumentDtoList = new List<InstrumentDto>();

            foreach (var instrument in instruments)
            {
                instrumentDtoList.Add(MapToDto(instrument));
            }

            return instrumentDtoList;
        }

        public InstrumentDto MapToDto(ISurvey instrument)
        {
            return new InstrumentDto
            {
                Name = instrument.Name,
                ServerParkName = instrument.ServerPark
            };
        }
    }
}
