namespace Blaise.Api.Storage.Interfaces
{
    using System.Threading.Tasks;

    public interface ICloudStorageService
    {
        Task<string> DownloadFileFromQuestionnaireBucketAsync(string filePath, string tempFilePath);

        Task DownloadFilesFromNisraBucketAsync(string folderPath, string tempFilePath);

        Task<string> DownloadFileFromIngestBucketAsync(string filePath, string tempFilePath);
    }
}
