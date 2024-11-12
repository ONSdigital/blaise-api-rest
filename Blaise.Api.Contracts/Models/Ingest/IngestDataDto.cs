
namespace Blaise.Api.Contracts.Models.Ingest
{
    public class IngestDataDto
    {
        public IngestDataDto(string bucketName, string bucketPath)
        {
            BucketName = bucketName;
            BucketPath = bucketPath;
        }

        public string BucketName { get; set; }
        public string BucketPath { get; set; }
    }
}
