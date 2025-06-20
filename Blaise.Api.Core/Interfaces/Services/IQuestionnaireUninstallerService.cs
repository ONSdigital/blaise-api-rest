namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IQuestionnaireUninstallerService
    {
        void UninstallQuestionnaire(string questionnaireName, string serverParkName);
    }
}