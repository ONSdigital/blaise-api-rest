namespace Blaise.Api.Core.Interfaces.Mappers
{
    using Blaise.Nuget.Api.Contracts.Enums;
    using StatNeth.Blaise.API.ServerManager;

    public interface IQuestionnaireStatusMapper
    {
        QuestionnaireStatusType GetQuestionnaireStatus(ISurvey questionnaire);
    }
}
