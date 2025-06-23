using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Ingest;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IIngestService
    {
        Task IngestDataAsync(IngestDataDto ingestDataDto, string serverParkName, string questionnaireName, string tempFilePath);
    }
}
