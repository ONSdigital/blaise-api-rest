
namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class StubConfigurationHelper
    {
        public static string StubbedApiUrl = "http://localhost:9443/";

        public static bool UseStubs =
            #if DEBUG
                true;
            #else
                false;
            #endif
    }
}
