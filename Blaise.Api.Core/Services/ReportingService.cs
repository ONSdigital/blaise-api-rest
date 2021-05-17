using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
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
                var fields = _blaiseCaseApi.GetRecordDataFields(cases.ActiveRecord);

                foreach (var fieldId in fieldIds)
                {
                    var field = fields
                        .FirstOrDefault(f => f.Key.Equals(fieldId, StringComparison.InvariantCultureIgnoreCase));

                    if (field.Key != null)
                    {
                        reportingData.Add(field.Key, field.Value);
                    }
                }

                reportDto.ReportingData.Add(reportingData);
                cases.MoveNext();
            }

            return reportDto;
        }
    }
}
