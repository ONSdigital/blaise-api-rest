using System.Threading.Tasks;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Tests.Behaviour.Stubs.IO
{
    public class CloudStorageServiceStub : ICloudStorageService
    {
        public Task<string> DownloadPackageFromQuestionnaireBucketAsync(string fileName, string tempFilePath)
        {
            return Task.FromResult("OpnTest.bkpg");
        }

        public Task DownloadDatabaseFilesFromNisraBucketAsync(string bucketPath, string tempFilePath)
        {
            return Task.FromResult("");
        }
    }
}
