using System.Threading.Tasks;

namespace Blaise.Api.Storage.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> DownloadPackageFromQuestionnaireBucketAsync(string fileName, string tempFilePath);
        Task DownloadDatabaseFilesFromNisraBucketAsync(string bucketPath, string tempFilePath);
        Task DownloadFilesFromBucketAsync(string bucketName, string bucketPath, string tempFilePath);
    }
}