using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseCatiApiStub : IBlaiseCatiApi
    {
        public IEnumerable<ISurvey> GetInstalledSurveys(string serverParkName)
        {
            throw new NotImplementedException();
        }

        public ISurvey GetInstalledSurvey(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public DayBatchModel CreateDayBatch(string instrumentName, string serverParkName, DateTime dayBatchDate,
            bool checkForTreatedCases)
        {
            throw new NotImplementedException();
        }

        public DayBatchModel GetDayBatch(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void AddToDayBatch(string instrumentName, string serverParkName, string primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> GetSurveyDays(string instrumentName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void SetSurveyDay(string instrumentName, string serverParkName, DateTime surveyDay)
        {
            throw new NotImplementedException();
        }

        public void SetSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays)
        {
            throw new NotImplementedException();
        }

        public void RemoveSurveyDay(string instrumentName, string serverParkName, DateTime surveyDay)
        {
            throw new NotImplementedException();
        }

        public void RemoveSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays)
        {
            throw new NotImplementedException();
        }
    }
}
