namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Threading.Tasks;
    using Blaise.Api.Contracts.Models.Questionnaire;

    public interface IQuestionnaireInstallerService
    {
        Task<string> InstallQuestionnaireAsync(string serverParkName, QuestionnairePackageDto questionnairePackageDto, string tempFilePath);
    }
}
