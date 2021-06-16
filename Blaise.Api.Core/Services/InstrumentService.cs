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
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IInstrumentDtoMapper _mapper;

        public InstrumentService(
            IBlaiseSurveyApi blaiseApi,
            IBlaiseCaseApi blaiseCaseApi,
            IInstrumentDtoMapper mapper)
        {
            _blaiseSurveyApi = blaiseApi;
            _blaiseCaseApi = blaiseCaseApi;
            _mapper = mapper;
        }

        public IEnumerable<InstrumentDto> GetAllInstruments()
        {
            var instruments = _blaiseSurveyApi.GetSurveysAcrossServerParks();
            return _mapper.MapToInstrumentDtos(instruments);
        }

        public IEnumerable<InstrumentDto> GetInstruments(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instruments = _blaiseSurveyApi.GetSurveys(serverParkName);
            return _mapper.MapToInstrumentDtos(instruments);
        }

        public InstrumentDto GetInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrument = _blaiseSurveyApi.GetSurvey(instrumentName, serverParkName);
            return _mapper.MapToInstrumentDto(instrument);
        }

        public bool InstrumentExists(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseSurveyApi.SurveyExists(instrumentName, serverParkName);
        }

        public Guid GetInstrumentId(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseSurveyApi.GetIdOfSurvey(instrumentName, serverParkName);
        }

        public SurveyStatusType GetInstrumentStatus(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseSurveyApi.GetSurveyStatus(instrumentName, serverParkName);
        }

        public DateTime? GetLiveDate(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseSurveyApi.GetLiveDate(instrumentName, serverParkName);
        }

        public void ActivateInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseSurveyApi.ActivateSurvey(instrumentName, serverParkName);
        }

        public void DeactivateInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseSurveyApi.DeactivateSurvey(instrumentName, serverParkName);
        }

        public IEnumerable<InstrumentUacDto> GetUacCodes(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var instrumentDtoList = new List<InstrumentUacDto>();
            var cases = _blaiseCaseApi.GetCases(instrumentName, serverParkName);

            while (!cases.EndOfSet)
            {
                var activeRecord = cases.ActiveRecord;
                instrumentDtoList.Add(new InstrumentUacDto
                {
                    CaseId = _blaiseCaseApi.GetPrimaryKeyValue(activeRecord),
                    UacCode = _blaiseCaseApi.GetFieldValue(activeRecord, "QDataBag.uac1").ValueAsText +
                              _blaiseCaseApi.GetFieldValue(activeRecord, "QDataBag.uac2").ValueAsText +
                              _blaiseCaseApi.GetFieldValue(activeRecord, "QDataBag.uac3").ValueAsText
                });

                cases.MoveNext();
            }

            return instrumentDtoList;
        }
    }
}
