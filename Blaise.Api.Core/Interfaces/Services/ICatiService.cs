using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICatiService
    {
        List<CatiInstrumentDto> GetCatiInstruments();

        List<CatiInstrumentDto> GetCatiInstruments(string serverParkName);

        CatiInstrumentDto GetCatiInstrument(string serverParkName, string instrumentName);

        DayBatchDto CreateDayBatch(string instrumentName, string serverParkName, CreateDayBatchDto createDayBatchDto);

        DayBatchDto GetDayBatch(string instrumentName, string serverParkName);

        void AddCasesToDayBatch(string instrumentName, string serverParkName, List<string> caseIds);

        List<DateTime> GetSurveyDays(string instrumentName, string serverParkName);

        List<DateTime> AddSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays);


        void RemoveSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays);
    }
}