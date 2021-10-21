using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IInstrumentDtoMapper _instrumentDtoMapper;
        private readonly IDataEntrySettingsDtoMapper _dataEntrySettingsDtoMapper;

        public InstrumentService(
            IBlaiseSurveyApi blaiseApi,
            IInstrumentDtoMapper instrumentDtoMapper, 
            IDataEntrySettingsDtoMapper dataEntryDtoMapper)
        {
            _blaiseApi = blaiseApi;
            _instrumentDtoMapper = instrumentDtoMapper;
            _dataEntrySettingsDtoMapper = dataEntryDtoMapper;
        }

        public IEnumerable<InstrumentDto> GetAllInstruments()
        {
            var instruments = _blaiseApi.GetSurveysAcrossServerParks();
            return _instrumentDtoMapper.MapToInstrumentDtos(instruments);
        }

        public IEnumerable<InstrumentDto> GetInstruments(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instruments = _blaiseApi.GetSurveys(serverParkName);
            return _instrumentDtoMapper.MapToInstrumentDtos(instruments);
        }

        public InstrumentDto GetInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrument = _blaiseApi.GetSurvey(instrumentName, serverParkName);
            return _instrumentDtoMapper.MapToInstrumentDto(instrument);
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

        public IEnumerable<string> GetModes(string instrumentName, string serverParkName)
        {
            return _blaiseApi.GetSurveyModes(instrumentName, serverParkName);
        }

        public bool ModeExists(string instrumentName, string serverParkName, string mode)
        {
            var modeList = _blaiseApi.GetSurveyModes(instrumentName, serverParkName);

            return modeList.Any(m => m.Equals(mode, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<DataEntrySettingsDto> GetDataEntrySettings(string instrumentName, string serverParkName)
        {
            var dataEntrySettingsModels = _blaiseApi.GetSurveyDataEntrySettings(instrumentName, serverParkName);

            return _dataEntrySettingsDtoMapper.MapDataEntrySettingsDtos(dataEntrySettingsModels);
        }
    }
}
