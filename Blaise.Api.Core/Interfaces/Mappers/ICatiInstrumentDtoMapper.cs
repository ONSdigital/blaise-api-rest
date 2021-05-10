using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface ICatiInstrumentDtoMapper
    {
        CatiInstrumentDto MapToCatiInstrumentDto(ISurvey instrument, List<DateTime> surveyDays,
            DateTime? liveDate);
    }
}