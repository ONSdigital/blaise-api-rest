﻿using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Instrument;
using Blaise.Api.Tests.Helpers.RestApi;
using Blaise.Api.Tests.Models.Questionnaire;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Blaise.Api.Tests.Behaviour.Steps
{
    [Binding]
    public sealed class QuestionnaireSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private const string ApiResponse = "ApiResponse";

        public QuestionnaireSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"There is an instrument installed on a Blaise environment")]
        public void GivenThereIsAnInstrumentInstalledOnABlaiseEnvironment()
        {
            InstrumentHelper.GetInstance().InstallInstrument();
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
        public async System.Threading.Tasks.Task WhenTheApiIsQueriedToReturnAllActiveQuestionnairesAsync()
        {
            var listOfActiveQuestionnaires = await RestApiHelper.GetInstance().GetAllActiveQuestionnaires();
            _scenarioContext.Set(listOfActiveQuestionnaires, ApiResponse);
        }

        [Then(@"details of questionnaire a is returned")]
        public void ThenDetailsOfQuestionnaireAIsReturned()
        {
            var listOfActiveQuestionnaires = _scenarioContext.Get<List<Questionnaire>>(ApiResponse);
            Assert.AreEqual(1, listOfActiveQuestionnaires.Count);
            Assert.IsTrue(listOfActiveQuestionnaires.Any(q => q.Name == BlaiseConfigurationHelper.InstrumentName));
        }

        [Then(@"an empty list is returned")]
        public void ThenAnEmptyListIsReturned()
        {
            var listOfActiveQuestionnaires = _scenarioContext.Get<List<Questionnaire>>(ApiResponse);
            Assert.AreEqual(0, listOfActiveQuestionnaires.Count);
        }

        [AfterScenario("questionnaires")]
        public void CleanUpScenario()
        {
            InstrumentHelper.GetInstance().UninstallSurvey();
        }
    }
}
