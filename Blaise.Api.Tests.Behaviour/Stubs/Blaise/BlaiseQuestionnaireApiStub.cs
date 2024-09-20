using System;
using System.Collections.Generic;
using Blaise.Api.Tests.Behaviour.Stubs.Responses;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseQuestionnaireApiStub : IBlaiseQuestionnaireApi
    {
        public bool QuestionnaireInstalled;
        public string NameOfInstalledQuestionnaire;
        public string ServerParkOfInstalledQuestionnaire;
        public QuestionnaireStatusType? QuestionnaireStatus;

        public bool QuestionnaireExists(string questionnaireName, string serverParkName)
        {
            return NameOfInstalledQuestionnaire.Equals(questionnaireName, StringComparison.CurrentCultureIgnoreCase) &&
                   ServerParkOfInstalledQuestionnaire.Equals(serverParkName, StringComparison.CurrentCultureIgnoreCase);
        }

        public IEnumerable<ISurvey> GetQuestionnairesAcrossServerParks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISurvey> GetQuestionnaires(string serverParkName)
        {
            return QuestionnaireResponses.ActiveQuestionnaires;
        }

        public ISurvey GetQuestionnaire(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public QuestionnaireStatusType GetQuestionnaireStatus(string questionnaireName, string serverParkName)
        {
            if (QuestionnaireInstalled && QuestionnaireStatus != null)
            {
                return (QuestionnaireStatusType)QuestionnaireStatus;
            }

            return QuestionnaireStatusType.Erroneous;
        }

        public IEnumerable<string> GetNamesOfQuestionnaires(string serverParkName)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdOfQuestionnaire(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void InstallQuestionnaire(string questionnaireName, string serverParkName, string questionnaireFile,
            IInstallOptions installOptions)
        {
            NameOfInstalledQuestionnaire = questionnaireName;
            ServerParkOfInstalledQuestionnaire = serverParkName;
            QuestionnaireInstalled = true;
            QuestionnaireStatus = QuestionnaireStatusType.Active;
        }

        public void UninstallQuestionnaire(string questionnaireName, string serverParkName, bool deleteCases = false)
        {
            NameOfInstalledQuestionnaire = null;
            ServerParkOfInstalledQuestionnaire = null;
            QuestionnaireInstalled = false;
            QuestionnaireStatus = null;
        }

        public void ActivateQuestionnaire(string questionnaireName, string serverParkName)
        {
            QuestionnaireStatus = QuestionnaireStatusType.Active;
        }

        public void DeactivateQuestionnaire(string questionnaireName, string serverParkName)
        {
            QuestionnaireStatus = QuestionnaireStatusType.Inactive;
        }

        public IEnumerable<string> GetQuestionnaireModes(string questionnaireName, string serverParkName)
        {
            return new List<string>
            {
                "CAWI",
                "CATI"
            };
        }

        public IEnumerable<DataEntrySettingsModel> GetQuestionnaireDataEntrySettings(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void UninstallQuestionnaire(string questionnaireName, string serverParkName, bool deleteCases = false, bool clearCati = false, bool dropTables = false)
        {
            throw new NotImplementedException();
        }

        public QuestionnaireConfigurationModel GetQuestionnaireConfigurationModel(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }
    }
}
