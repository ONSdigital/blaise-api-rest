using System;
using System.Threading;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Stubs;
using Blaise.Nuget.Api.Api;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Tests.Behaviour.Helpers.Questionnaire
{
    public class QuestionnaireHelper
    {
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;

        private static QuestionnaireHelper _currentInstance;

        public QuestionnaireHelper()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                _blaiseQuestionnaireApi = UnityConfigStub.Resolve<IBlaiseQuestionnaireApi>();

                return;
            }

            _blaiseQuestionnaireApi = new BlaiseQuestionnaireApi();
        }

        public static QuestionnaireHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new QuestionnaireHelper());
        }

        public void InstallQuestionnaire()
        {
            if (QuestionnaireDoesNotExist(BlaiseConfigurationHelper.QuestionnaireName, 60))
            {
                _blaiseQuestionnaireApi.InstallQuestionnaire(
                    BlaiseConfigurationHelper.QuestionnaireName,
                    BlaiseConfigurationHelper.ServerParkName,
                    BlaiseConfigurationHelper.QuestionnairePackagePath,
                    QuestionnaireInterviewType.Cati);

                    return;
            }

            throw new Exception(
                "There has been an issue where the instrument has not been cleared down, and still exists");
        }

        public bool QuestionnaireHasInstalled(int timeoutInSeconds)
        {
            return QuestionnaireExists(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds) &&
                   QuestionnaireIsActive(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds);
        }

        public void UninstallQuestionnaire()
        {
            _blaiseQuestionnaireApi.UninstallQuestionnaire(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        public bool SetQuestionnaireAsActive(int timeoutInSeconds)
        {
            _blaiseQuestionnaireApi.ActivateQuestionnaire(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);

            return QuestionnaireIsActive(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds);
        }

        private bool QuestionnaireIsActive(string questionnaireName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (GetQuestionnaireStatus(questionnaireName) == QuestionnaireStatusType.Installing)
            {
                Thread.Sleep(timeoutInSeconds % maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return GetQuestionnaireStatus(questionnaireName) == QuestionnaireStatusType.Active;
        }

        private bool QuestionnaireExists(string questionnaireName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (!_blaiseQuestionnaireApi.QuestionnaireExists(questionnaireName, BlaiseConfigurationHelper.ServerParkName))
            {
                Thread.Sleep(timeoutInSeconds % maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private bool QuestionnaireDoesNotExist(string questionnaireName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (_blaiseQuestionnaireApi.QuestionnaireExists(questionnaireName, BlaiseConfigurationHelper.ServerParkName))
            {
                Thread.Sleep(timeoutInSeconds % maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private QuestionnaireStatusType GetQuestionnaireStatus(string questionnaireName)
        {
            return _blaiseQuestionnaireApi.GetQuestionnaireStatus(questionnaireName, BlaiseConfigurationHelper.ServerParkName);
        }
    }
}
