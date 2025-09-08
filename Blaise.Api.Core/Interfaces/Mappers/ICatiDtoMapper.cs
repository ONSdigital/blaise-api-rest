namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System;
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Cati;
    using Blaise.Nuget.Api.Contracts.Models;
    using StatNeth.Blaise.API.ServerManager;

    public interface ICatiDtoMapper
    {
        CatiQuestionnaireDto MapToCatiQuestionnaireDto(ISurvey questionnaire, List<DateTime> surveyDays);

        DayBatchDto MapToDayBatchDto(DayBatchModel dayBatchModel);
    }
}
