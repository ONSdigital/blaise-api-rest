using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public ReportingService(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }

        public ReportDto GetReportingData(string serverParkName, string instrumentName,
            string[] fieldIds)
        {
            var reportDto = new ReportDto();
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
