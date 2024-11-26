using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class RestApiConfigurationHelper
    {
        public static string BaseUrl => ConfigurationExtensions.GetVariable("ENV_RESTAPI_URL");

        public static string QuestionnairesUrl =>
            $"/api/v2/serverparks/{BlaiseConfigurationHelper.ServerParkName}/questionnaires";

        public static string QuestionnaireDataUrl =>
            $"/api/v2/serverparks/{BlaiseConfigurationHelper.ServerParkName}/questionnaires/{BlaiseConfigurationHelper.QuestionnaireName}/data";

        public static string IngestDataUrl =>
            $"/api/v2/serverparks/{BlaiseConfigurationHelper.ServerParkName}/questionnaires/{BlaiseConfigurationHelper.QuestionnaireName}/ingest";
    }
}
