using System;
using System.IO;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Case;
using Blaise.Api.Tests.Helpers.Cloud;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Extensions;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class DeliverQuestionnaireWithDataSteps
    {
        private const int ExpectedNumberOfCases = 10;
        private const string ApiResponse = "ApiResponse";

        private readonly ScenarioContext _scenarioContext;

        public DeliverQuestionnaireWithDataSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
        
        [Then(@"the questionnaire does not contain any correspondent data")]
        public void ThenTheQuestionnaireDoesNotContainAnyCorrespondentData()
        {
            var numberOfCases = CaseHelper.GetInstance().NumberOfCasesInInstrument();
            Assert.AreEqual(0, numberOfCases);
        }

        [Given(@"we have captured correspondent data for the questionnaire")]
        public void GivenWeHaveCapturedCorrespondentDataForTheQuestionnaire()
        {
            CaseHelper.GetInstance().CreateCases(ExpectedNumberOfCases);
        }

        [When(@"the API is called to deliver the questionnaire with data")]
        public async Task WhenTheApiIsCalledToDeliverTheQuestionnaireWithData()
        {
            var deliveredFile = await RestApiHelper.GetInstance().DeliverInstrumentWithData(
                RestApiConfigurationHelper.InstrumentDataDeliveryUrl,
                BlaiseConfigurationHelper.InstrumentBucketPath);

            _scenarioContext.Set(deliveredFile, ApiResponse);
        }

        [Then(@"the questionnaire package is delivered to the bucket")]
        public void ThenTheQuestionnairePackageIsDeliveredToTheBucket()
        {
            var deliveredFile = _scenarioContext.Get<string>(ApiResponse);
            var fileName = Path.GetFileName(deliveredFile);

            var exists = CloudStorageHelper.GetInstance().FileExists(
                BlaiseConfigurationHelper.DeliveredInstrumentBucketPath, fileName);

            Assert.True(exists);
        }

        [Then(@"the questionnaire is package uses the agreed file name format")]
        public void ThenTheQuestionnaireIsPackageUsesTheAgreedFileNameFormat()
        {
            var deliveredFile = _scenarioContext.Get<string>(ApiResponse);
            var dateTime = DateTime.Now;
            var expectedPartialFileName = $"dd_{BlaiseConfigurationHelper.InstrumentName}_{dateTime:ddMMyyyy}_";
            var fileName = Path.GetFileName(deliveredFile);

            Assert.True(fileName.StartsWith(expectedPartialFileName));
        }

        [Then(@"the questionnaire package contains the captured correspondent data")]
        public async Task ThenTheQuestionnairePackageContainsTheCapturedCorrespondentData()
        {
            var deliveredFile = _scenarioContext.Get<string>(ApiResponse);
            var fileName = Path.GetFileName(deliveredFile);
            var destinationFilePath = Path.Combine(BlaiseConfigurationHelper.TempDownloadPath, fileName);
            
            var downloadedFile = await CloudStorageHelper.GetInstance().DownloadFromBucketAsync(
                BlaiseConfigurationHelper.DeliveredInstrumentBucketPath,
                fileName,
                destinationFilePath);

            var extractedFilePath = Path.GetDirectoryName(destinationFilePath);

            downloadedFile.ExtractFile(extractedFilePath);
            var dataInterfaceFile = $@"{extractedFilePath}\{BlaiseConfigurationHelper.InstrumentName}.bdix";
            var numberOfCases = CaseHelper.GetInstance().NumberOfCasesInInstrument(dataInterfaceFile);

            Assert.AreEqual(ExpectedNumberOfCases, numberOfCases);
        }
        
        [AfterScenario("deliver")]
        public async Task CleanUpScenario()
        {
            CaseHelper.GetInstance().DeleteCases();
            InstrumentHelper.GetInstance().UninstallSurvey();

            var fileName = Path.GetFileName(BlaiseConfigurationHelper.InstrumentPackage);

            await CloudStorageHelper.GetInstance().DeleteFromBucketAsync(
                BlaiseConfigurationHelper.InstrumentBucketPath,
                fileName);

            Directory.Delete(BlaiseConfigurationHelper.TempDownloadPath, true);
        }
    }
}
