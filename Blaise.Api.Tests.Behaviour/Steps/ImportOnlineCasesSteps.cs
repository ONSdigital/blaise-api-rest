﻿using System;
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
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public class ImportOnlineCasesSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private static string _tempFilePath;

        public ImportOnlineCasesSteps(ScenarioContext scenarioContext)
        {
            _tempFilePath = Path.Combine(BlaiseConfigurationHelper.TempPath, "Tests", Guid.NewGuid().ToString());
            _scenarioContext = scenarioContext;
        }

        [BeforeFeature("onlinedata")]
        public static void SetupUpFeature()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            QuestionnaireHelper.GetInstance().InstallQuestionnaire();
        }

        [Given(@"there is a online file that contains the following cases")]
        public async Task GivenThereIsAOnlineFileThatContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            await OnlineFileHelper.GetInstance().CreateCasesInOnlineFileAsync(cases, _tempFilePath);
        }

        [Given(@"blaise contains the following cases")]
        public void GivenBlaiseContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            CaseHelper.GetInstance().CreateCasesInBlaise(cases);
        }

        [When(@"the online file is processed")]
        public async Task WhenTheOnlineFileIsProcessed()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var statusCode = await RestApiHelper.GetInstance().ImportOnlineCases(RestApiConfigurationHelper.QuestionnaireDataUrl,
                BlaiseConfigurationHelper.QuestionnaireName);

            Assert.AreEqual(HttpStatusCode.Created, statusCode);
        }

        [Then(@"blaise will contain the following cases")]
        public void ThenBlaiseWillContainTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

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

                Assert.AreEqual(caseRecordExpected.Outcome, caseModel.Outcome, $"expected an outcome of '{caseRecordExpected.Outcome}' for case '{caseModel.PrimaryKey}'," +
                                                                               $"but was '{caseModel.Outcome}'");

                Assert.AreEqual(caseRecordExpected.Mode, caseModel.Mode, $"expected an version of '{caseRecordExpected.Mode}' for case '{caseModel.PrimaryKey}'," +
                                                                         $"but was '{caseModel.Mode}'");
            }
        }

        [Given(@"there is a online file that contains a case that is complete")]
        public async Task GivenThereIsAOnlineDataFileThatContainsACaseThatIsCompleteAsync()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(110);
        }

        [Given(@"there is a online file that contains a case that is partially complete")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatIsPartiallyComplete()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(210);
        }

        [Given(@"there is a online file that contains a case that has not been started")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatHasNotBeenStarted()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            await GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(0);
        }

        [Given(@"there is an online file that contains a case with the outcome code '(.*)'")]
        public async Task GivenThereIsAnOnlineFileThatContainsACaseWithTheOutcomeCodeAsync(int outcomeCode)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = await OnlineFileHelper.GetInstance().CreateCaseInOnlineFileAsync(outcomeCode, _tempFilePath);
            _scenarioContext.Set(primaryKey,"primaryKey");
        }

        [Given(@"the same case exists in Blaise that is complete")]
        public void GivenTheSameCaseExistsInBlaiseThatIsComplete()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(110);
        }

        [Given(@"the same case exists in Blaise that is partially complete")]
        public void GivenTheSameCaseExistsInBlaiseThatIsPartiallyComplete()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(210);
        }

        [Given(@"there is a online file that contains a case that has previously been imported")]
        public async Task GivenThereIsAOnlineFileThatContainsACaseThatHasPreviouslyBeenImported()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var caseModel = CaseHelper.GetInstance().CreateCaseModel("110", ModeType.Web, DateTime.Now);
            CaseHelper.GetInstance().CreateCaseInBlaise(caseModel);

            caseModel.Mode = ModeType.Ftf; //used to differentiate the case to ensure it has not been imported again
            await OnlineFileHelper.GetInstance().CreateCaseInOnlineFileAsync(caseModel, _tempFilePath);
            
            _scenarioContext.Set(caseModel.PrimaryKey,"primaryKey");
        }
        
        [Given(@"the same case exists in Blaise with the outcome code '(.*)'")]
        public void GivenTheSameCaseExistsInBlaiseWithTheOutcomeCode(int outcomeCode)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var caseModel = new CaseModel(primaryKey, outcomeCode.ToString(), ModeType.Tel, DateTime.Now.AddHours(-2));
            CaseHelper.GetInstance().CreateCaseInBlaise(caseModel);
        }

        [Given(@"the case has been updated within the past 30 minutes")]
        public void GivenTheCaseIsCurrentlyOpenInCati()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            CaseHelper.GetInstance().MarkCaseAsOpenInCati(primaryKey);
        }

        [Then(@"the existing blaise case is overwritten with the online case")]
        public void ThenTheExistingBlaiseCaseIsOverwrittenWithTheOnlineCase()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.AreEqual(ModeType.Web, modeType);
        }

        [Then(@"the existing blaise case is kept")]
        public void ThenTheExistingBlaiseCaseIsKept()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.AreEqual(ModeType.Tel, modeType);
        }

        [Then(@"the online case is not imported again")]
        public void ThenTheOnlineCaseIsNotImportedAgain()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var primaryKey = _scenarioContext.Get<string>("primaryKey");
            var modeType = CaseHelper.GetInstance().GetMode(primaryKey);
            Assert.AreEqual(ModeType.Web, modeType);
        }

        [Given(@"there is a online file that contains '(.*)' cases")]
        public async Task GivenThereIsAOnlineFileThatContainsCases(int numberOfCases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            await OnlineFileHelper.GetInstance().CreateCasesInOnlineFileAsync(numberOfCases, _tempFilePath);
        }
        
        [Given(@"blaise contains no cases")]
        public void GivenTheBlaiseDatabaseIsEmpty()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            CaseHelper.GetInstance().DeleteCases();
        }

        [Given(@"blaise contains '(.*)' cases")]
        public void GivenBlaiseContainsCases(int numberOfCases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            CaseHelper.GetInstance().CreateCasesInBlaise(numberOfCases);
        }
        
        [Then(@"blaise will contain no cases")]
        public void ThenBlaiseWillContainNoCases()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            ThenCasesWillBeImportedIntoBlaise(0);
        }

        [Then(@"blaise will contain '(.*)' cases")]
        public void ThenCasesWillBeImportedIntoBlaise(int numberOfCases)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var numberOfCasesInBlaise = CaseHelper.GetInstance().NumberOfCasesInQuestionnaire();

            Assert.AreEqual(numberOfCases, numberOfCasesInBlaise);
        }

        [AfterScenario("onlinedata")]
        public static async Task CleanUpScenario()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            CaseHelper.GetInstance().DeleteCases();
            await OnlineFileHelper.GetInstance().CleanUpOnlineFiles();
            FileSystemHelper.GetInstance().CleanUpTempFiles(_tempFilePath);
        }
        
        [AfterFeature("onlinedata")]
        public static void CleanUpFeature()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire();
        }
    }
}
