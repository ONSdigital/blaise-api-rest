using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class SharedSteps
    {
        [BeforeTestRun]
        public static void StartApiIfRunningLocally()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                RestApiHelper.GetInstance().StartWebApi();
            }
        }

        [AfterTestRun]
        public static void StopApiIfRunningLocally()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                RestApiHelper.GetInstance().StopWebApi();
            }
        }
    }
}
