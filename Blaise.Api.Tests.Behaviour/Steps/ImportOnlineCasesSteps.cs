namespace Blaise.Api.Tests.Behaviour.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Blaise.Api.Tests.Behaviour.Helpers.Case;
    using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
    using Blaise.Api.Tests.Behaviour.Helpers.Files;
    using Blaise.Api.Tests.Behaviour.Helpers.Questionnaire;
    using Blaise.Api.Tests.Behaviour.Helpers.RestApi;
    using Blaise.Api.Tests.Behaviour.Models.Case;
    using Blaise.Api.Tests.Behaviour.Models.Enums;
    using NUnit.Framework;
    using Reqnroll;

    [Binding]
    public class ImportOnlineCasesSteps
    {
        private static string _tempFilePath;
        private readonly ScenarioContext _scenarioContext;

        public ImportOnlineCasesSteps(ScenarioContext scenarioContext)
        {
            _tempFilePath = Path.Combine(BlaiseConfigurationHelper.TempPath, "Tests", Guid.NewGuid().ToString());
            _scenarioContext = scenarioContext;
        }

        [BeforeFeature("onlinedata")]
        public static void SetupUpFeature()
        {
            QuestionnaireHelper.GetInstance().InstallQuestionnaire();
        }

        [AfterScenario("onlinedata")]
        public static async Task CleanUpScenario()
        {
            CaseHelper.GetInstance().DeleteCases();
            await OnlineFileHelper.GetInstance().CleanUpOnlineFiles();
            FileSystemHelper.GetInstance().CleanUpTempFiles(_tempFilePath);
        }

        [AfterFeature("onlinedata")]
        public static void CleanUpFeature()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire(60);
        }

        [Given("there is a online file that contains the following cases")]
        public async Task GivenThereIsAOnlineFileThatContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            await OnlineFileHelper.GetInstance().CreateCasesInOnlineFileAsync(cases, _tempFilePath);
        }

        [Given("blaise contains the following cases")]
        public void GivenBlaiseContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            CaseHelper.GetInstance().CreateCasesInBlaise(cases);
        }

        [When("the online file is processed")]
        public async Task WhenTheOnlineFileIsProcessed()
        {
            var statusCode = await RestApiHelper.GetInstance().ImportOnlineCases(
                RestApiConfigurationHelper.QuestionnaireDataUrl,
                BlaiseConfigurationHelper.QuestionnaireName);

            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.Accepted));
        }

        [Then("blaise will contain the following cases")]
        public void ThenBlaiseWillContainTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            var numberOfCasesInDatabase = CaseHelper.GetInstance().NumberOfCasesInQuestionnaire();
            var casesExpected = cases.ToList();

            if (casesExpected.Count != numberOfCasesInDatabase)
            {
                Assert.Fail($"Expected '{casesExpected.Count}' cases in the database, but {numberOfCasesInDatabase} cases were found");
            }

            var casesInDatabase = CaseHelper.GetInstance().GetCasesInDatabase();

            foreach (var caseModel in casesInDatabase)
            {
                var caseRecordExpected = casesExpected.FirstOrDefault(c => c.PrimaryKey == caseModel.PrimaryKey);

                if (caseRecordExpected == null)
                {
                    Assert.Fail($"Case {caseModel.PrimaryKey} was in the database but not found in expected cases");
                }

                Assert.That(
                    caseModel.Outcome,
                    Is.EqualTo(caseRecordExpected.Outcome),
                    $"expected an outcome of '{caseRecordExpected.Outcome}' for case '{caseModel.PrimaryKey}', but was '{caseModel.Outcome}'");

                Assert.That(
                    caseRecordExpected.Mode,
                    Is.EqualTo(caseModel.Mode),
                    $"expected an version of '{caseRecordExpected.Mode}' for case '{caseModel.PrimaryKey}', but was '{caseModel.Mode}'");
            }
        }

        [Given("there is a online file that contains a case that is complete")]
        public async Task GivenThereIsAOnlineDataFileThatContainsACaseThatIsCompleteAsync()
        {
            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(110);
        }

        [Given("there is a online file that contains a case that is partially complete")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatIsPartiallyComplete()
        {
            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(210);
        }

        [Given("there is a online file that contains a case that is ineligible")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatIsIneligible()
        {
            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(580);
        }

        [Given("there is a online file that contains a case that has not been started")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatHasNotBeenStarted()
        {
            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(0);
        }

        [Given("there is an online file that contains a case with the outcome code '(.*)'")]
        public async Task GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(int outcomeCode)
        {
            var primaryKey = await OnlineFileHelper.GetInstance().CreateCaseInOnlineFileAsync(outcomeCode, _tempFilePath);
            _scenarioContext.Set(primaryKey, "primaryKey");
        }

        [Given("the same case exists in Blaise that is complete")]
        public void GivenTheSameCaseExistsInBlaiseThatIsComplete()
        {
            GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(110);
        }

        [Given("the same case exists in Blaise that is partially complete")]
        public void GivenTheSameCaseExistsInBlaiseThatIsPartiallyComplete()
        {
            GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(210);
        }

        [Given("there is a online file that contains a case that has previously been imported")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatHasPreviouslyBeenImported()
        {
            var caseModel = CaseHelper.GetInstance().CreateCaseModel("110", ModeType.Web, DateTime.Now);
            CaseHelper.GetInstance().CreateCaseInBlaise(caseModel);

            caseModel.Mode = ModeType.Ftf; // used to differentiate the case to ensure it has not been imported again
            await OnlineFileHelper.GetInstance().CreateCaseInOnlineFileAsync(caseModel, _tempFilePath);

            _scenarioContext.Set(caseModel.PrimaryKey, "primaryKey");
        }

        [Given("the same case exists in Blaise with the outcome code '(.*)'")]
        public void GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(int outcomeCode)
        {
            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var caseModel = new CaseModel(primaryKey, outcomeCode.ToString(), ModeType.Tel, DateTime.Now.AddHours(-2));
            CaseHelper.GetInstance().CreateCaseInBlaise(caseModel);
        }

        [Given("the case has been updated within the past 30 minutes")]
        public void GivenTheCaseIsCurrentlyOpenInCati()
        {
            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            CaseHelper.GetInstance().MarkCaseAsOpenInCati(primaryKey);
        }

        [Then("the existing blaise case is overwritten with the online case")]
        public void ThenTheExistingBlaiseCaseIsOverwrittenWithTheOnlineCase()
        {
            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.That(ModeType.Web, Is.EqualTo(modeType));
        }

        [Then("the existing blaise case is kept")]
        public void ThenTheExistingBlaiseCaseIsKept()
        {
            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.That(ModeType.Tel, Is.EqualTo(modeType));
        }

        [Then("the online case is not imported again")]
        public void ThenTheOnlineCaseIsNotImportedAgain()
        {
            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.That(ModeType.Web, Is.EqualTo(modeType));
        }

        [Given("there is a online file that contains '(.*)' cases")]
        public async Task GivenThereIsAOnlineFileThatContainsCases(int numberOfCases)
        {
            await OnlineFileHelper.GetInstance().CreateCasesInOnlineFileAsync(numberOfCases, _tempFilePath);
        }

        [Given("blaise contains no cases")]
        public void GivenTheBlaiseDatabaseIsEmpty()
        {
            CaseHelper.GetInstance().DeleteCases();
        }

        [Given("blaise contains '(.*)' cases")]
        public void GivenBlaiseContainsCases(int numberOfCases)
        {
            CaseHelper.GetInstance().CreateCasesInBlaise(numberOfCases);
        }

        [Then("blaise will contain no cases")]
        public void ThenBlaiseWillContainNoCases()
        {
            ThenCasesWillBeImportedIntoBlaise(0);
        }

        [Then("blaise will contain '(.*)' cases")]
        public void ThenCasesWillBeImportedIntoBlaise(int numberOfCases)
        {
            var numberOfCasesInBlaise = CaseHelper.GetInstance().NumberOfCasesInQuestionnaire();

            Assert.That(numberOfCases, Is.EqualTo(numberOfCasesInBlaise));
        }
    }
}
