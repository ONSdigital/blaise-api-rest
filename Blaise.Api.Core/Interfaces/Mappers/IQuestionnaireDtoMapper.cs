using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IQuestionnaireDtoMapper
    {
        IEnumerable<QuestionnaireDto> MapToQuestionnaireDtos(IEnumerable<ISurvey> questionnaires);

        QuestionnaireDto MapToQuestionnaireDto(ISurvey questionnaire);
    }
}
