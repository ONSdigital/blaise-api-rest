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
        public bool SurveyExists(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        }

        public void UninstallSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void ActivateSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void DeactivateSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSurveyModes(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataEntrySettingsModel> GetSurveyDataEntrySettings(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }
    }
}
