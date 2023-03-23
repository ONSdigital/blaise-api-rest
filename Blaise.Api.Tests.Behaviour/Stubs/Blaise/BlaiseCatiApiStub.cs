using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseCatiApiStub : IBlaiseCatiApi
    {
        public IEnumerable<ISurvey> GetInstalledQuestionnaires(string serverParkName)
        {
            throw new NotImplementedException();
        }

        public ISurvey GetInstalledQuestionnaire(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public DayBatchModel CreateDayBatch(string questionnaireName, string serverParkName, DateTime dayBatchDate,
            bool checkForTreatedCases)
        {
            throw new NotImplementedException();
        }

        public DayBatchModel GetDayBatch(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void AddToDayBatch(string questionnaireName, string serverParkName, string primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        public List<DateTime> GetSurveyDays(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void SetSurveyDay(string questionnaireName, string serverParkName, DateTime surveyDay)
        {
            throw new NotImplementedException();
        }

        public void SetSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays)
        {
            throw new NotImplementedException();
        }

        public void RemoveSurveyDay(string questionnaireName, string serverParkName, DateTime surveyDay)
        {
            throw new NotImplementedException();
        }

        public void RemoveSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays)
        {
            throw new NotImplementedException();
        }

        public bool MakeSuperAppointment(string questionnaireName, string serverParkName, string primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        public int ClearAppointments(string questionnaireName, string serverParkName, List<string> primaryKeys)
        {
            throw new NotImplementedException();
        }

        public int CreateAppointment(string questionnaireName, string serverParkName, string primaryKey, DateTime appointmentDateTime,
            string notes = null, int updateMode = 1, string toWhom = null)
        {
            throw new NotImplementedException();
            //test
        }
    }
}
