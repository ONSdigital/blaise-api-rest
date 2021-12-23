using System;
using System.Collections.Generic;
using Blaise.Api.Tests.Behaviour.Stubs.Responses;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseSurveyApiStub : IBlaiseSurveyApi
    {
        public bool SurveyInstalled;
        public string NameOfInstalledSurvey;
        public string ServerParkOfInstalledSurvey;
        public SurveyStatusType? SurveyStatus;

        public bool SurveyExists(string instrumentName, string serverParkName)
        {
            return NameOfInstalledSurvey.Equals(instrumentName, StringComparison.CurrentCultureIgnoreCase) &&
                   ServerParkOfInstalledSurvey.Equals(serverParkName, StringComparison.CurrentCultureIgnoreCase);
        }

        public IEnumerable<ISurvey> GetSurveysAcrossServerParks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISurvey> GetSurveys(string serverParkName)
        {
            return InstrumentResponses.ActiveQuestionnaires;
        }

        public ISurvey GetSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public SurveyStatusType GetSurveyStatus(string instrumentName, string serverParkName)
        {
            if (SurveyInstalled && SurveyStatus != null)
            {
                return (SurveyStatusType)SurveyStatus;
            }

            return SurveyStatusType.Erroneous;
        }

        public IEnumerable<string> GetNamesOfSurveys(string serverParkName)
        {
            throw new NotImplementedException();
        }

        public SurveyInterviewType GetSurveyInterviewType(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdOfSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void InstallSurvey(string instrumentName, string serverParkName, string instrumentFile,
            SurveyInterviewType surveyInterviewType)
        {
            NameOfInstalledSurvey = instrumentName;
            ServerParkOfInstalledSurvey = serverParkName;
            SurveyInstalled = true;
            SurveyStatus = SurveyStatusType.Active;
        }

        public void UninstallSurvey(string instrumentName, string serverParkName)
        {
            NameOfInstalledSurvey = null;
            ServerParkOfInstalledSurvey = null;
            SurveyInstalled = false;
            SurveyStatus = null;
        }

        public void ActivateSurvey(string instrumentName, string serverParkName)
        {
            SurveyStatus = SurveyStatusType.Active;
        }

        public void DeactivateSurvey(string instrumentName, string serverParkName)
        {
            SurveyStatus = SurveyStatusType.Inactive;
        }

        public IEnumerable<string> GetSurveyModes(string instrumentName, string serverParkName)
        {
            return new List<string>
            {
                "CAWI",
                "CATI"
            };
        }

        public IEnumerable<DataEntrySettingsModel> GetSurveyDataEntrySettings(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }
    }
}
