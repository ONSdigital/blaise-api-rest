namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Threading.Tasks;
    using Blaise.Api.Contracts.Models.Questionnaire;

    public interface IQuestionnaireDataService
    {
        Task ImportOnlineDataAsync(QuestionnaireDataDto questionnaireDataDto, string serverParkName, string questionnaireName, string tempFilePath);
    }
}
