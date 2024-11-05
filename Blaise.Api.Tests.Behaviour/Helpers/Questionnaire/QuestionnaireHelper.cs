using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Stubs;
using Blaise.Nuget.Api.Api;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Extensions;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;
using System;
using System.Threading;
using Blaise.Nuget.Api.Contracts.Extensions;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

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
            var installOptions = new InstallOptions
            {
                DataEntrySettingsName = QuestionnaireDataEntryType.StrictInterviewing.ToString(),
                InitialAppLayoutSetGroupName = QuestionnaireInterviewType.Cati.FullName(),
                LayoutSetGroupName = QuestionnaireInterviewType.Cati.FullName(),
                OverwriteMode = DataOverwriteMode.Always,
            };

            _blaiseQuestionnaireApi.InstallQuestionnaire(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName,
                BlaiseConfigurationHelper.QuestionnairePackagePath,
                installOptions);
        }

        public bool QuestionnaireHasInstalled(int timeoutInSeconds)
        {
            return QuestionnaireExists(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds) &&
                   QuestionnaireIsActive(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds);
        }

        public void UninstallQuestionnaire(int timeoutInSeconds)
        {
            _blaiseQuestionnaireApi.UninstallQuestionnaire(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);

            if (!QuestionnaireHasBeenUninstalled(BlaiseConfigurationHelper.QuestionnaireName, timeoutInSeconds))
            {

                throw new Exception($"It appears the questionnaire '{BlaiseConfigurationHelper.QuestionnaireName}' has not uninstalled successfully");
            }
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
                Thread.Sleep((timeoutInSeconds * 1000) % maxCount);

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
                Thread.Sleep(timeoutInSeconds * 1000 / maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private bool QuestionnaireHasBeenUninstalled(string questionnaireName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (_blaiseQuestionnaireApi.QuestionnaireExists(questionnaireName, BlaiseConfigurationHelper.ServerParkName))
            {
                Thread.Sleep(timeoutInSeconds * 1000 / maxCount);

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
