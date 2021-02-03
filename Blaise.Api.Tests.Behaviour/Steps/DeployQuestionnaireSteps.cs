using System.IO;
using System.Net;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Cloud;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Files;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using NUnit.Framework;
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
                BlaiseConfigurationHelper.InstrumentPackageBucket,
                BlaiseConfigurationHelper.InstrumentPackage);
        }

        [Given(@"the API is called to deploy the questionnaire")]
        [When(@"the API is called to deploy the questionnaire")]
        public async Task WhenTheApiIsCalledToDeployTheQuestionnaire()
        {
            var response = await RestApiHelper.GetInstance().DeployQuestionnaire(
                RestApiConfigurationHelper.InstrumentsUrl,
                BlaiseConfigurationHelper.InstrumentFile);

            Assert.AreEqual(HttpStatusCode.Created, response);
        }
        
        [AfterScenario("deploy")]
        public async Task CleanUpScenario()
        {
            InstrumentHelper.GetInstance().UninstallSurvey();
            
            var fileName = Path.GetFileName(BlaiseConfigurationHelper.InstrumentPackage);
            
            await CloudStorageHelper.GetInstance().DeleteFromBucketAsync(
                BlaiseConfigurationHelper.InstrumentPackageBucket,
                fileName);

            FileSystemHelper.GetInstance().CleanUpTempFiles();
        }
    }
}
