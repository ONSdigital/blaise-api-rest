using System.Threading.Tasks;

namespace Blaise.Api.Storage.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> DownloadFromBucketAsync(string fileName);
        Task UploadToBucketAsync(string bucketPath, string filePath);
    }
}