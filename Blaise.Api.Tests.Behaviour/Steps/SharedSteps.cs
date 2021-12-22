using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class SharedSteps
    {
        [BeforeScenario]
        public void StartApiIfRunningLocally()
        {
            if (RestApiConfigurationHelper.RunLocalUsingStubs)
            {
                RestApiHelper.GetInstance().StartWebApi();
            }
        }

        [AfterScenario]
        public void StopApiIfRunningLocally()
        {
            if (RestApiConfigurationHelper.RunLocalUsingStubs)
            {
                RestApiHelper.GetInstance().StopWebApi();
            }
        }
    }
}
