namespace Blaise.Api.Tests.Behaviour.Steps
{
    using System.Net;
    using System.Threading.Tasks;
    using Blaise.Api.Tests.Behaviour.Helpers.Cloud;
    using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
    using Blaise.Api.Tests.Behaviour.Helpers.Questionnaire;
    using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
    using NUnit.Framework;
    using Reqnroll;

    [Binding]
    public sealed class DeployQuestionnaireSteps
    {
        [Given("I have a questionnaire I want to install")]
        public async Task GivenIHaveAQuestionnaireIWantToInstall()
        {
            await CloudStorageHelper.GetInstance().UploadFileToBucketAsync(
                BlaiseConfigurationHelper.QuestionnairePackageBucket,
                BlaiseConfigurationHelper.QuestionnairePackagePath);
        }

        [When("the API is called to install the questionnaire")]
        public async Task WhenTheApiIsCalledToInstallTheQuestionnaire()
        {
            var response = await RestApiHelper.GetInstance().DeployQuestionnaire(
                RestApiConfigurationHelper.QuestionnairesUrl,
                BlaiseConfigurationHelper.QuestionnaireFile);

            Assert.That(response, Is.EqualTo(HttpStatusCode.Created));
        }

        [Then("the questionnaire is available to use")]
        public void ThenTheQuestionnaireIsAvailableToUse()
        {
            var questionnaireHasInstalled = QuestionnaireHelper.GetInstance().QuestionnaireHasInstalled(60);

            Assert.That(questionnaireHasInstalled, Is.True, "The questionnaire has not been installed, or is not active");
        }

        [AfterScenario("deploy")]
        public async Task CleanUpScenario()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire(60);

            await CloudStorageHelper.GetInstance().DeleteFileInBucketAsync(
                BlaiseConfigurationHelper.QuestionnairePackageBucket,
                BlaiseConfigurationHelper.QuestionnairePackage);
        }
    }
}
