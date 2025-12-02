namespace Blaise.Api.Tests.Behaviour.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
    using Blaise.Api.Tests.Behaviour.Helpers.Questionnaire;
    using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
    using Blaise.Api.Tests.Behaviour.Models.Questionnaire;
    using NUnit.Framework;
    using Reqnroll;

    [Binding]
    public sealed class GetQuestionnaireDetailsSteps
    {
        private const string ApiResponse = "ApiResponse";
        private readonly ScenarioContext _scenarioContext;

        public GetQuestionnaireDetailsSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [AfterScenario("questionnaires")]
        public static void CleanUpScenario()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire(60);
        }

        [Given("there is a questionnaire installed on a Blaise environment")]
        public void GivenThereIsAQuestionnaireInstalledOnABlaiseEnvironment()
        {
            QuestionnaireHelper.GetInstance().InstallQuestionnaire();
            Assert.That(QuestionnaireHelper.GetInstance().QuestionnaireHasInstalled(60), Is.True);
        }

        [Given("the questionnaire is active")]
        public void GivenTheQuestionnaireIsActive()
        {
            var surveyIsActive = QuestionnaireHelper.GetInstance().SetQuestionnaireAsActive(60);
            Assert.That(surveyIsActive, Is.True);
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
            Assert.That(listOfActiveQuestionnaires.Any(q => q.Name == BlaiseConfigurationHelper.QuestionnaireName), Is.True);
        }
    }
}
