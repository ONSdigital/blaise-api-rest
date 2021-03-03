using System;
using System.IO;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Case;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Extensions;
using Blaise.Api.Tests.Helpers.Files;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class GetQuestionnaireDataSteps
    {
        private const int ExpectedNumberOfCases = 10;
        private const string ApiResponse = "ApiResponse";
        private readonly string _tempFilePath;

        private readonly ScenarioContext _scenarioContext;

        public GetQuestionnaireDataSteps(ScenarioContext scenarioContext)
        {
            _tempFilePath = Path.Combine(BlaiseConfigurationHelper.TempPath, "Tests", Guid.NewGuid().ToString());
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
            CaseHelper.GetInstance().CreateCasesInBlaise(ExpectedNumberOfCases);
        }

        [When(@"the API is called to retrieve the questionnaire with data")]
        public async Task WhenTheApiIsCalledToDeliverTheQuestionnaireWithData()
        {
            var instrumentFile = await RestApiHelper.GetInstance().GetInstrumentWithData(
                RestApiConfigurationHelper.InstrumentDataUrl, _tempFilePath);

            _scenarioContext.Set(instrumentFile, ApiResponse);
        }

        [Then(@"the questionnaire package contains the captured correspondent data")]
        public void ThenTheQuestionnairePackageContainsTheCapturedCorrespondentData()
        {
            var deliveredFile = _scenarioContext.Get<string>(ApiResponse);
            var extractedFilePath = Path.Combine(
                _tempFilePath, 
                BlaiseConfigurationHelper.InstrumentName);

            deliveredFile.ExtractFiles(extractedFilePath);
            var dataInterfaceFile = $@"{extractedFilePath}\{BlaiseConfigurationHelper.InstrumentName}.bdix";
            var numberOfCases = CaseHelper.GetInstance().NumberOfCasesInInstrument(dataInterfaceFile);

            Assert.AreEqual(ExpectedNumberOfCases, numberOfCases);
        }

        [AfterScenario("data")]
        public void CleanUpScenario()
        {
            CaseHelper.GetInstance().DeleteCases();
            InstrumentHelper.GetInstance().UninstallSurvey();
            FileSystemHelper.GetInstance().CleanUpTempFiles(_tempFilePath);
        }
    }
}
