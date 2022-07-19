using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Questionnaire;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IQuestionnaireInstallerService
    {
        Task<string> InstallQuestionnaireAsync(string serverParkName, QuestionnairePackageDto questionnairePackageDto, string tempFilePath);
    }
}