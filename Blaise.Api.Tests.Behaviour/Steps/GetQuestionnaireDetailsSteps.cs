using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Files;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using Blaise.Api.Tests.Models.Questionnaire;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class GetQuestionnaireDetailsSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private const string ApiResponse = "ApiResponse";

        public GetQuestionnaireDetailsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"there is a questionnaire installed on a Blaise environment")]
        public void GivenThereIsAnInstrumentInstalledOnABlaiseEnvironment()
        {
            InstrumentHelper.GetInstance().InstallInstrument();
            Assert.IsTrue(InstrumentHelper.GetInstance().SurveyHasInstalled(60));
        }

        [Given(@"the questionnaire is active")]
        public void GivenTheQuestionnaireIsActive()
        {
            var surveyIsActive = InstrumentHelper.GetInstance().SetSurveyAsActive(60);
            Assert.IsTrue(surveyIsActive);
        }

        [Given(@"the questionnaire is inactive")]
        public void GivenTheQuestionnaireIsInactive()
        {
            var surveyIsInactive = InstrumentHelper.GetInstance().SetSurveyAsInactive();
            Assert.IsTrue(surveyIsInactive);
        }

        [Given(@"there are no questionnaires installed")]
        public void GivenThereAreNoQuestionnairesInstalled()
        {
        }

        [When(@"the API is queried to return all active questionnaires")]
        public async Task WhenTheApiIsQueriedToReturnAllActiveQuestionnairesAsync()
        {
            var listOfActiveQuestionnaires =  await RestApiHelper.GetInstance().GetAllActiveQuestionnaires();
            _scenarioContext.Set(listOfActiveQuestionnaires, ApiResponse);
        }

        [Then(@"the details of the questionnaire is returned")]
        public void ThenDetailsOfQuestionnaireAIsReturned()
        {
            var listOfActiveQuestionnaires = _scenarioContext.Get<List<Questionnaire>>(ApiResponse);
            Assert.IsTrue(listOfActiveQuestionnaires.Any(q => q.Name == BlaiseConfigurationHelper.InstrumentName));
        }

        [Then(@"an empty list is returned")]
        public void ThenAnEmptyListIsReturned()
        {
            var listOfActiveQuestionnaires = _scenarioContext.Get<List<Questionnaire>>(ApiResponse);
            Assert.AreEqual(0, listOfActiveQuestionnaires.Count);
        }

        [Then(@"the questionnaire is available to use in the Blaise environment")]
        public void ThenTheInstrumentIsAvailableToUseInTheBlaiseEnvironment()
        {
            var instrumentHasInstalled = InstrumentHelper.GetInstance().SurveyHasInstalled(60);

            Assert.IsTrue(instrumentHasInstalled, "The instrument has not been installed, or is not active");
        }

        [AfterFeature("questionnaires")]
        public static void CleanUpScenario()
        {
            InstrumentHelper.GetInstance().UninstallSurvey();
            FileSystemHelper.GetInstance().CleanUpTempFiles();
        }
    }
}
