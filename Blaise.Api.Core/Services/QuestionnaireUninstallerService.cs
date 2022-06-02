using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class QuestionnaireUninstallerService : IQuestionnaireUninstallerService
    {
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public QuestionnaireUninstallerService(
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi, 
            IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
            _blaiseCaseApi = blaiseCaseApi;
        }
        public void UninstallQuestionnaire(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseCaseApi.RemoveCases(questionnaireName, serverParkName);
            _blaiseQuestionnaireApi.UninstallQuestionnaire(questionnaireName, serverParkName);
        }
    }
}
