using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Questionnaire;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IIngestService
    {
        Task IngestDataAsync(QuestionnaireDataDto questionnaireDataDto, string serverParkName, string questionnaireName, string tempFilePath);
    }
}