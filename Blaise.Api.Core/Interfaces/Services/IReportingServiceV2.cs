using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Reports;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IReportingServiceV2
    {
        ReportV2Dto GetReportingData(string serverParkName, string questionnaireName,
            List<string> fieldIds);

        ReportV2Dto GetReportingData(string serverParkName, Guid questionnaireId, List<string> fieldIds);
    }
}