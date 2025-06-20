using Blaise.Nuget.Api.Contracts.Enums;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IQuestionnaireStatusMapper
    {
        QuestionnaireStatusType GetQuestionnaireStatus(ISurvey questionnaire);
    }
}