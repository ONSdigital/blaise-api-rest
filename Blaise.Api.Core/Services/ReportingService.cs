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
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;

        public ReportingService(
            IBlaiseCaseApi blaiseCaseApi,
            IBlaiseSurveyApi blaiseSurveyApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseSurveyApi = blaiseSurveyApi;
        }

        public ReportDto GetReportingData(string serverParkName, string instrumentName,
            List<string> fieldIds)
        {
            var instrumentId = _blaiseSurveyApi.GetIdOfSurvey(instrumentName, serverParkName);

            return BuildReportDto(serverParkName, instrumentName, instrumentId, fieldIds);
        }

        public ReportDto GetReportingData(string serverParkName, Guid instrumentId, List<string> fieldIds)
        {
            var surveys = _blaiseSurveyApi.GetSurveys(serverParkName);
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
            var caseFields = _blaiseCaseApi.GetRecordDataFields(caseRecord);

            foreach (var fieldId in fieldIds)
            {
                if (caseFields.Any(f => f.Key.Equals(fieldId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    reportingData.Add(fieldId, caseFields.FirstOrDefault(f => f.Key.Equals(fieldId, StringComparison.InvariantCultureIgnoreCase)).Value);
                }
            }

            return reportingData;
        }
    }
}