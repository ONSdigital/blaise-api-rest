using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface ICatiDtoMapper
    {
        CatiInstrumentDto MapToCatiInstrumentDto(ISurvey instrument, List<DateTime> surveyDays);

        DayBatchDto MapToDayBatchDto(DayBatchModel dayBatchModel);
    }
}