using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

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
                var reportingData = new Dictionary<string, string>();

                foreach (var fieldId in fieldIds)
                {
                    reportingData.Add(fieldId, _blaiseCaseApi.GetFieldValue(cases.ActiveRecord, fieldId).ValueAsText);
                }

                _blaiseCaseApi.GetFieldValue(cases.ActiveRecord, FieldNameType.HOut).ValueAsText);

                reportDto.ReportingData.Add(reportingData);
                cases.MoveNext();
            }

            return reportDto;
        }
    }
}
