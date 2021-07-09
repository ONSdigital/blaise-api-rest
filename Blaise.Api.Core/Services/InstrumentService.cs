using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentService : IInstrumentService
    {
        private readonly IBlaiseSurveyApi _blaiseApi;
        private readonly IInstrumentDtoMapper _mapper;

        public InstrumentService(
            IBlaiseSurveyApi blaiseApi,
            IInstrumentDtoMapper mapper)
        {
            _blaiseApi = blaiseApi;
            _mapper = mapper;
        }

        public IEnumerable<InstrumentDto> GetAllInstruments()
        {
            var instruments = _blaiseApi.GetSurveysAcrossServerParks();
            return _mapper.MapToInstrumentDtos(instruments);
        }

        public IEnumerable<InstrumentDto> GetInstruments(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instruments = _blaiseApi.GetSurveys(serverParkName);
            return _mapper.MapToInstrumentDtos(instruments);
        }

        public InstrumentDto GetInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrument = _blaiseApi.GetSurvey(instrumentName, serverParkName);
            return _mapper.MapToInstrumentDto(instrument);
        }

        public bool InstrumentExists(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseApi.SurveyExists(instrumentName, serverParkName);
        }

        public Guid GetInstrumentId(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseApi.GetIdOfSurvey(instrumentName, serverParkName);
        }

        public SurveyStatusType GetInstrumentStatus(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseApi.GetSurveyStatus(instrumentName, serverParkName);
        }

        public void ActivateInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseApi.ActivateSurvey(instrumentName, serverParkName);
        }

        public void DeactivateInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseApi.DeactivateSurvey(instrumentName, serverParkName);
        }
    }
}
