namespace Blaise.Api.Core.Interfaces.Services
{
    using System;
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Reports;

    public interface IReportingService
    {
        ReportDto GetReportingData(
            string serverParkName,
            string questionnaireName,
            List<string> fieldIds,
            string filter);

        ReportDto GetReportingData(string serverParkName, Guid questionnaireId, List<string> fieldIds, string filter);
    }
}
