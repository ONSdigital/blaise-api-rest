using System.IO;
using System.Threading.Tasks;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Google.Cloud.Storage.V1;

namespace Blaise.Api.Tests.Behaviour.Helpers.Cloud
{
    public class CloudStorageHelper
    {
        private StorageClient _storageClient;
        private static CloudStorageHelper _currentInstance;

        public static CloudStorageHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new CloudStorageHelper());
        }

        public async Task UploadToBucketAsync(string bucketPath, string filePath)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var storageClient = GetStorageClient();
            using (var fileStream = File.OpenRead(filePath))
            {
                await storageClient.UploadObjectAsync(bucketPath, Path.GetFileName(filePath), null, fileStream);
            }
        }

        public async Task UploadFolderToBucketAsync(string bucketPath, string folderPath)
        {
            var storageClient = GetStorageClient();
            var filesInFolder = Directory.GetFiles(folderPath);
            foreach (var file in filesInFolder)
            {
                using (var fileStream = File.OpenRead(file))
                {
                    await storageClient.UploadObjectAsync(bucketPath, $"{BlaiseConfigurationHelper.InstrumentName}/{Path.GetFileName(file)}", null, fileStream);
                }
            }          
        }

        public async Task DeleteFileInBucketAsync(string bucketPath, string fileName)
        {
            if (StubConfigurationHelper.UseStubs)
            {
                return;
            }

            var storageClient = GetStorageClient();
            await storageClient.DeleteObjectAsync(bucketPath, fileName);
        }

        public async Task DeleteFilesInBucketAsync(string bucketName, string bucketPath)
        {
            var storageClient = GetStorageClient();
            var storageObjects = storageClient.ListObjects(bucketName, $"{bucketPath}/");

            foreach (var storageObject in storageObjects)
            {
                await storageClient.DeleteObjectAsync(bucketName, storageObject.Name);
            }
        }

        private StorageClient GetStorageClient()
        {
            var client = _storageClient;

            if (client != null)
            {
                return client;
            }

            return _storageClient = StorageClient.Create();
        }
    }
}
