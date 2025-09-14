namespace Blaise.Api.Core.Services
{
    using Blaise.Api.Core.Extensions;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Nuget.Api.Contracts.Interfaces;

    public class QuestionnaireUninstallerService : IQuestionnaireUninstallerService
    {
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseSqlApi _blaiseSqlApi;

        public QuestionnaireUninstallerService(
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi,
            IBlaiseCaseApi blaiseCaseApi,
            IBlaiseSqlApi blaiseSqlApi)
        {
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseSqlApi = blaiseSqlApi;
        }

        public void UninstallQuestionnaire(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseCaseApi.RemoveCases(questionnaireName, serverParkName);
            _blaiseQuestionnaireApi.UninstallQuestionnaire(questionnaireName, serverParkName);

            _blaiseSqlApi.DropQuestionnaireTables(questionnaireName);
        }
    }
}
