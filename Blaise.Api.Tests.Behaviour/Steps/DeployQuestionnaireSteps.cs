using System.IO;
using System.Net;
using System.Threading.Tasks;
using Blaise.Api.Tests.Behaviour.Helpers.Cloud;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Helpers.Instrument;
using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class DeployQuestionnaireSteps
    {
        [Given(@"I have a questionnaire I want to install")]
        public async Task GivenIHaveAQuestionnaireIWantToInstall()
        {
            await CloudStorageHelper.GetInstance().UploadToBucketAsync(
                BlaiseConfigurationHelper.InstrumentPackageBucket,
                BlaiseConfigurationHelper.InstrumentPackage);

        }

        [When(@"the API is called to install the questionnaire")]
        public async Task WhenTheApiIsCalledToInstallTheQuestionnaire()
        {
            var response = await RestApiHelper.GetInstance().DeployQuestionnaire(
                RestApiConfigurationHelper.InstrumentsUrl,
                BlaiseConfigurationHelper.InstrumentFile);

            Assert.AreEqual(HttpStatusCode.Created, response);
        }

        [Then(@"the questionnaire is available to use")]
        public void ThenTheQuestionnaireIsAvailableToUse()
        {
            var instrumentHasInstalled = InstrumentHelper.GetInstance().SurveyHasInstalled(60);

            Assert.IsTrue(instrumentHasInstalled, "The instrument has not been installed, or is not active");
        }

        [AfterScenario("deploy")]
        public async Task CleanUpScenario()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            InstrumentHelper.GetInstance().UninstallSurvey();

            var fileName = Path.GetFileName(BlaiseConfigurationHelper.InstrumentPackage);

            await CloudStorageHelper.GetInstance().DeleteFileInBucketAsync(
                BlaiseConfigurationHelper.InstrumentPackageBucket,
                fileName);
        }
    }
}
