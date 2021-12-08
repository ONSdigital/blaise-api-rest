using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;
using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Exceptions;

// ReSharper disable PossibleInvalidOperationException

namespace Blaise.Api.Core.Services
{
    public class CatiService : ICatiService
    {
        private readonly IBlaiseCatiApi _blaiseCatiApi;
        private readonly IBlaiseServerParkApi _blaiseServerParkApi;
        private readonly ICatiDtoMapper _mapper;

        public CatiService(
            IBlaiseCatiApi blaiseApi,
            IBlaiseServerParkApi blaiseServerParkApi,
            ICatiDtoMapper mapper)
        {
            _blaiseCatiApi = blaiseApi;
            _blaiseServerParkApi = blaiseServerParkApi;
            _mapper = mapper;
        }

        public List<CatiInstrumentDto> GetCatiInstruments()
        {
            var catiInstruments = new List<CatiInstrumentDto>();
            var serverParks = _blaiseServerParkApi.GetNamesOfServerParks();

            foreach (var serverPark in serverParks)
            {
                var instruments = _blaiseCatiApi.GetInstalledSurveys(serverPark);
                catiInstruments.AddRange(GetCatiInstruments(instruments));
            }

            return catiInstruments;
        }

        public List<CatiInstrumentDto> GetCatiInstruments(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instruments = _blaiseCatiApi.GetInstalledSurveys(serverParkName);

            return GetCatiInstruments(instruments);
        }

        public CatiInstrumentDto GetCatiInstrument(string serverParkName, string instrumentName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrument = _blaiseCatiApi.GetInstalledSurvey(instrumentName, serverParkName);

            return GetCatiInstrumentDto(instrument);
        }

        public DayBatchDto CreateDayBatch(string instrumentName, string serverParkName, CreateDayBatchDto createDayBatchDto)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            createDayBatchDto.ThrowExceptionIfNull("createDayBatchDto");
            createDayBatchDto.DayBatchDate.ThrowExceptionIfNull("createDayBatchDto.DayBatchDate");
            createDayBatchDto.CheckForTreatedCases.ThrowExceptionIfNull("createDayBatchDto.CheckForTreatedCases");

            var dayBatchModel = _blaiseCatiApi.CreateDayBatch(instrumentName, serverParkName,
                (DateTime)createDayBatchDto.DayBatchDate, (bool)createDayBatchDto.CheckForTreatedCases);

            return _mapper.MapToDayBatchDto(dayBatchModel);
        }

        public DayBatchDto GetDayBatch(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var dayBatchModel = _blaiseCatiApi.GetDayBatch(instrumentName, serverParkName);

            if (dayBatchModel == null)
            {
                throw new DataNotFoundException("No daybatch found");
            }

            return _mapper.MapToDayBatchDto(dayBatchModel);
        }

        public bool InstrumentHasADayBatchForToday(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var dayBatchModel = _blaiseCatiApi.GetDayBatch(instrumentName, serverParkName);

            return dayBatchModel?.DayBatchDate.Date == DateTime.Today;
        }

        public void AddCasesToDayBatch(string instrumentName, string serverParkName, List<string> caseIds)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            caseIds.ThrowExceptionIfNullOrEmpty("caseIds");

            foreach (var caseId in caseIds)
            {
                _blaiseCatiApi.AddToDayBatch(instrumentName, serverParkName, caseId);
            }
        }

        public List<DateTime> GetSurveyDays(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var surveyDays = _blaiseCatiApi.GetSurveyDays(instrumentName, serverParkName);

            return surveyDays;
        }

        public List<DateTime> AddSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            surveyDays.ThrowExceptionIfNullOrEmpty("surveyDays");

            _blaiseCatiApi.SetSurveyDays(instrumentName, serverParkName, surveyDays);

            return GetSurveyDays(instrumentName, serverParkName);
        }

        public void RemoveSurveyDays(string instrumentName, string serverParkName, List<DateTime> surveyDays)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            surveyDays.ThrowExceptionIfNullOrEmpty("surveyDays");

            _blaiseCatiApi.RemoveSurveyDays(instrumentName, serverParkName, surveyDays);
        }

        private List<CatiInstrumentDto> GetCatiInstruments(IEnumerable<ISurvey> instruments)
        {
            var catiInstruments = new List<CatiInstrumentDto>();

            foreach (var instrument in instruments)
            {
                catiInstruments.Add(GetCatiInstrumentDto(instrument));
            }

            return catiInstruments;
        }

        private CatiInstrumentDto GetCatiInstrumentDto(ISurvey instrument)
        {
            var surveyDays = _blaiseCatiApi.GetSurveyDays(instrument.Name, instrument.ServerPark);

            return _mapper.MapToCatiInstrumentDto(instrument, surveyDays);
        }
    }
}
