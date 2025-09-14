namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Threading.Tasks;
    using Blaise.Api.Contracts.Models.Ingest;

    public interface IIngestService
    {
        Task IngestDataAsync(IngestDataDto ingestDataDto, string serverParkName, string questionnaireName, string tempFilePath);
    }
}
