using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Services
{
    public class CatiService : ICatiService
    {
        private readonly IBlaiseCatiApi _blaiseCatiApi;
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;
        private readonly ICatiDtoMapper _mapper;

        public CatiService(
            IBlaiseCatiApi blaiseApi,
            IBlaiseSurveyApi blaiseSurveyApi,
            ICatiDtoMapper mapper
           )
        {
            _blaiseCatiApi = blaiseApi;
            _mapper = mapper;
            _blaiseSurveyApi = blaiseSurveyApi;
        }

        public List<CatiInstrumentDto> GetCatiInstruments()
        {
            var catiInstruments = new List<CatiInstrumentDto>();

            var instruments = _blaiseSurveyApi.GetSurveysAcrossServerParks();
            catiInstruments.AddRange(GetCatiInstruments(instruments));

            return catiInstruments;
        }

        public List<CatiInstrumentDto> GetCatiInstruments(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instruments = _blaiseSurveyApi.GetSurveys(serverParkName);

            return GetCatiInstruments(instruments);
        }

        public CatiInstrumentDto GetCatiInstrument(string serverParkName, string instrumentName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrument = _blaiseSurveyApi.GetSurvey(instrumentName, serverParkName);

            return GetCatiInstrumentDto(instrument);
        }

        public DayBatchDto CreateDayBatch(string instrumentName, string serverParkName, CreateDayBatchDto createDayBatchDto)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            createDayBatchDto.ThrowExceptionIfNull("createDayBatchDto");

            var dayBatchModel = _blaiseCatiApi.CreateDayBatch(instrumentName, serverParkName, 
                createDayBatchDto.DayBatchDate, createDayBatchDto.CheckForTreatedCases);

            return _mapper.MapToDayBatchDto(dayBatchModel);
        }

        public DayBatchDto GetDayBatch(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var dayBatchModel = _blaiseCatiApi.GetDayBatch(instrumentName, serverParkName);

            return _mapper.MapToDayBatchDto(dayBatchModel);
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
