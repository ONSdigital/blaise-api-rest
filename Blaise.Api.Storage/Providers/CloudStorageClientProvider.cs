using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Blaise.Api.Storage.Interfaces;
using Google.Cloud.Storage.V1;

namespace Blaise.Api.Storage.Providers
{
    public class CloudStorageClientProvider : ICloudStorageClientProvider
    {
        private readonly IFileSystem _fileSystem;

        public CloudStorageClientProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task DownloadAsync(string bucketName, string fileName, string destinationFilePath)
        {
            using (var storageClient = await StorageClient.CreateAsync())
            {
                using (var fileStream = _fileSystem.FileStream.Create(destinationFilePath, FileMode.OpenOrCreate))
                {
                    await storageClient.DownloadObjectAsync(bucketName, fileName, fileStream);
                }
            }
        }

        public async Task UploadAsync(string bucketName, string filePath)
        {
            using (var storageClient = await StorageClient.CreateAsync())
            {
                var fileName = Path.GetFileName(filePath);

                using (var fileStream = _fileSystem.FileStream.Create(filePath, FileMode.OpenOrCreate))
                {
                    await storageClient.UploadObjectAsync(bucketName, fileName, null, fileStream);
                }
            }
        }
    }
}
