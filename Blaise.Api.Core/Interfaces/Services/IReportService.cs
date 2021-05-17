using Blaise.Api.Contracts.Models.Reports;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IReportService
    {
        ReportDto GetReportingData(string serverParkName, string instrumentName,
            string[] fieldIds);
    }
}