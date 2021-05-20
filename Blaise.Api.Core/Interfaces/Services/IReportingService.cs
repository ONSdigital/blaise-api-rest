using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Reports;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IReportingService
    {
        ReportDto GetReportingData(string serverParkName, string instrumentName,
            List<string> fieldIds);
    }
}