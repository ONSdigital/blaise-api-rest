using System.Threading.Tasks;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Tests.Behaviour.Stubs.IO
{
    public class CloudStorageServiceStub : ICloudStorageService
    {
        public Task<string> DownloadFileFromQuestionnaireBucketAsync(string filePath, string tempFilePath)
        {
            return Task.FromResult("OpnTest.bkpg");
        }

        public Task DownloadFilesFromNisraBucketAsync(string folderPath, string tempFilePath)
        {
            return Task.FromResult("");
        }

        public Task<string> DownloadFileFromIngestBucketAsync(string filePath, string tempFilePath)
        {
            return Task.FromResult("");
        }
    }
}
