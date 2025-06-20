
namespace Blaise.Api.Contracts.Models.Ingest
{
    public class IngestDataDto
    {
        public IngestDataDto(string bucketFilePath)
        {
            BucketFilePath = bucketFilePath;
        }

        public string BucketFilePath { get; set; }
    }
}
