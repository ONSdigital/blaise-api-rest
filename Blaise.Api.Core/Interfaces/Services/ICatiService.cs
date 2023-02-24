using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Mappers;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICatiService
    {
        List<CatiQuestionnaireDto> GetCatiQuestionnaires();

        List<CatiQuestionnaireDto> GetCatiQuestionnaires(string serverParkName);

        CatiQuestionnaireDto GetCatiQuestionnaire(string serverParkName, string questionnaireName);

        DayBatchDto CreateDayBatch(string questionnaireName, string serverParkName, CreateDayBatchDto createDayBatchDto);

        DayBatchDto GetDayBatch(string questionnaireName, string serverParkName);

        bool QuestionnaireHasADayBatchForToday(string questionnaireName, string serverParkName);
        
        void AddCasesToDayBatch(string questionnaireName, string serverParkName, List<string> caseIds);

        List<DateTime> GetSurveyDays(string questionnaireName, string serverParkName);

        List<DateTime> AddSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays);

        void RemoveSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays);

        int ClearAppointments(string questionnaireName, string serverParkName);

        int CreateAppointment(AppointmentDto appointment);
    }
}