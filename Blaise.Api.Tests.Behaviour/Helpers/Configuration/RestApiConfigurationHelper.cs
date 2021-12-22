using System;
using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class RestApiConfigurationHelper
    {
        private const string LocalApiUrl = "http://localhost:9443/";

        public static bool RunLocalUsingStubs => Convert.ToBoolean(ConfigurationExtensions.GetVariable("USE_STUBS"));

        public static string BaseUrl => RunLocalUsingStubs ? LocalApiUrl : ConfigurationExtensions.GetVariable("ENV_RESTAPI_URL");

        public static string InstrumentsUrl =>
            $"/api/v1/serverparks/{BlaiseConfigurationHelper.ServerParkName}/instruments";

        public static string InstrumentDataUrl =>
            $"/api/v1/serverparks/{BlaiseConfigurationHelper.ServerParkName}/instruments/{BlaiseConfigurationHelper.InstrumentName}/data";
    }
}
