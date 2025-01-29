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
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public class IngestDataSteps
    {
        private static string _tempFilePath;

        public IngestDataSteps()
        {
            _tempFilePath = Path.Combine(BlaiseConfigurationHelper.TempPath, "Tests", Guid.NewGuid().ToString());
        }

        [BeforeFeature("ingest")]
        public static void SetupUpFeature()
        {
            QuestionnaireHelper.GetInstance().InstallQuestionnaire();
        }

        [Given("there is an ingest file that contains the following cases")]
        public async Task GivenThereIsAnIngestFileThatContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            await IngestFileHelper.GetInstance().CreateCasesInIngestFileAsync(cases, _tempFilePath);
        }

        [Given("blaise contains the existing cases")]
        public void GivenBlaiseContainsTheFollowingCases(IEnumerable<CaseModel> cases)
        {
            CaseHelper.GetInstance().CreateCasesInBlaise(cases);
        }

        [When("the ingest file is processed")]
        public async Task WhenTheIngestFileIsProcessed()
        {
            var statusCode = await RestApiHelper.GetInstance().IngestData(RestApiConfigurationHelper.IngestDataUrl,
                IngestFileHelper.IngestFile);

            Assert.AreEqual(HttpStatusCode.Created, statusCode);
        }

        [Then("blaise will contain the cases")]
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

                Assert.AreEqual(caseRecordExpected.Outcome, caseModel.Outcome, $"expected an outcome of '{caseRecordExpected.Outcome}' for case '{caseModel.PrimaryKey}'," +
                                                                               $"but was '{caseModel.Outcome}'");

                Assert.AreEqual(caseRecordExpected.Mode, caseModel.Mode, $"expected an version of '{caseRecordExpected.Mode}' for case '{caseModel.PrimaryKey}'," +
                                                                         $"but was '{caseModel.Mode}'");
            }
        }

        [AfterScenario("ingest")]
        public static void CleanUpScenario()
        {
            CaseHelper.GetInstance().DeleteCases();
            FileSystemHelper.GetInstance().CleanUpTempFiles(_tempFilePath);
        }
        
        [AfterFeature("ingest")]
        public static void CleanUpFeature()
        {
            QuestionnaireHelper.GetInstance().UninstallQuestionnaire(60);
        }
    }
}
