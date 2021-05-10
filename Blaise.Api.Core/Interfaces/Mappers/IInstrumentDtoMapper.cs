using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IInstrumentDtoMapper
    {
        IEnumerable<InstrumentDto> MapToInstrumentDtos(IEnumerable<ISurvey> instruments);
        InstrumentDto MapToInstrumentDto(ISurvey instrument);
    }
}