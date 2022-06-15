using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class RestApiConfigurationHelper
    {
        public static bool UseStubs => StubConfigurationHelper.UseStubs;

        public static string BaseUrl => UseStubs ? StubConfigurationHelper.StubbedApiUrl : ConfigurationExtensions.GetVariable("ENV_RESTAPI_URL");

        public static string QuestionnairesUrl =>
            $"/api/v1/serverparks/{BlaiseConfigurationHelper.ServerParkName}/instruments";

        public static string QuestionnaireDataUrl =>
            $"/api/v1/serverparks/{BlaiseConfigurationHelper.ServerParkName}/instruments/{BlaiseConfigurationHelper.QuestionnaireName}/data";
    }
}
