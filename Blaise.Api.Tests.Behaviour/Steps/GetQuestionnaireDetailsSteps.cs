using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Helpers.Questionnaire;
using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
using Blaise.Api.Tests.Behaviour.Models.Questionnaire;
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

        [Given("there is a questionnaire installed on a Blaise environment")]
        public void GivenThereIsAQuestionnaireInstalledOnABlaiseEnvironment()
        {
            QuestionnaireHelper.GetInstance().InstallQuestionnaire();
            Assert.IsTrue(QuestionnaireHelper.GetInstance().QuestionnaireHasInstalled(60));
        }

        [Given("the questionnaire is active")]
        public void GivenTheQuestionnaireIsActive()
        {
            var surveyIsActive = QuestionnaireHelper.GetInstance().SetQuestionnaireAsActive(60);
            Assert.IsTrue(surveyIsActive);
        }

        [When("the API is queried to return all active questionnaires")]
        public async Task WhenTheApiIsQueriedToReturnAllActiveQuestionnairesAsync()
        {
            var listOfActiveQuestionnaires = await RestApiHelper.GetInstance().GetAllActiveQuestionnaires();
            _scenarioContext.Set(listOfActiveQuestionnaires, ApiResponse);
        }

        [Then("the details of the questionnaire are returned")]
        public void ThenDetailsOfQuestionnaireAIsReturned()
        {
            var listOfActiveQuestionnaires = _scenarioContext.Get<List<QuestionnaireModel>>(ApiResponse);
            Assert.IsTrue(listOfActiveQuestionnaires.Any(q => q.Name == BlaiseConfigurationHelper.QuestionnaireName));
        }

        [AfterScenario("questionnaires")]
        public static void CleanUpScenario()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire(60);
        }
    }
}
