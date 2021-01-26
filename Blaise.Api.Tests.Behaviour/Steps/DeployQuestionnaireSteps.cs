using System.IO;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Cloud;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class DeployQuestionnaireSteps
    {
        [Given(@"there is a questionnaire available in a bucket")]
        public async Task GivenThereIsAnQuestionnaireAvailableInABucket()
        {
            await CloudStorageHelper.GetInstance().UploadToBucketAsync(
                BlaiseConfigurationHelper.InstrumentBucketPath,
                BlaiseConfigurationHelper.InstrumentPackage);
        }

        [Given(@"the API is called to deploy the questionnaire")]
        [When(@"the API is called to deploy the questionnaire")]
        public async Task WhenTheApiIsCalledToDeployTheQuestionnaire()
        {
            await RestApiHelper.GetInstance().DeployQuestionnaire(
                RestApiConfigurationHelper.InstrumentsUrl,
                BlaiseConfigurationHelper.InstrumentBucketPath,
                BlaiseConfigurationHelper.InstrumentPackage);
        }


        [AfterScenario("deploy")]
        public async Task CleanUpScenario()
        {
            InstrumentHelper.GetInstance().UninstallSurvey();
            
            var fileName = Path.GetFileName(BlaiseConfigurationHelper.InstrumentPackage);
            
            await CloudStorageHelper.GetInstance().DeleteFromBucketAsync(
                BlaiseConfigurationHelper.InstrumentBucketPath,
                fileName);
        }
    }
}
