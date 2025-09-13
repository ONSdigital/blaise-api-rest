namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using StatNeth.Blaise.API.ServerManager;

    public interface IQuestionnaireDtoMapper
    {
        IEnumerable<QuestionnaireDto> MapToQuestionnaireDtos(IEnumerable<ISurvey> questionnaires);

        QuestionnaireDto MapToQuestionnaireDto(ISurvey questionnaire);
    }
}
