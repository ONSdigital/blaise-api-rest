using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;

        public ReportingService(
            IBlaiseCaseApi blaiseCaseApi,
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
        }

        public ReportDto GetReportingData(string serverParkName, string instrumentName,
            List<string> fieldIds)
        {
            var instrumentId = _blaiseQuestionnaireApi.GetIdOfQuestionnaire(instrumentName, serverParkName);

            return BuildReportDto(serverParkName, instrumentName, instrumentId, fieldIds);
        }

        public ReportDto GetReportingData(string serverParkName, Guid instrumentId, List<string> fieldIds)
        {
            var surveys = _blaiseQuestionnaireApi.GetQuestionnaires(serverParkName);
            var instrumentName = surveys.First(s => s.InstrumentID == instrumentId).Name;

            return BuildReportDto(serverParkName, instrumentName, instrumentId, fieldIds);
        }

        private ReportDto BuildReportDto(string serverParkName, string instrumentName, Guid instrumentId, List<string> fieldIds)
        {
            var reportDto = new ReportDto
            {
                InstrumentName = instrumentName,
                InstrumentId = instrumentId
            };

            var cases = _blaiseCaseApi.GetCases(instrumentName, serverParkName);

            while (!cases.EndOfSet)
            {
                var caseData = GetReportFieldData(fieldIds, cases.ActiveRecord);

                reportDto.ReportingData.Add(caseData);

                cases.MoveNext();
            }

            return reportDto;
        }

        private Dictionary<string, string> GetReportFieldData(IEnumerable<string> fieldIds, IDataRecord caseRecord)
        {
            var reportingData = new Dictionary<string, string>();

            foreach (var fieldId in fieldIds)
            {
                reportingData.Add(fieldId, _blaiseCaseApi.GetFieldValue(caseRecord, fieldId).ValueAsText);
            }

            return reportingData;
        }
    }
}