using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Storage.Services
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICloudStorageClientProvider _cloudStorageClient;
        private readonly IFileSystem _fileSystem;

        public CloudStorageService(
            IConfigurationProvider configurationProvider,
            ICloudStorageClientProvider cloudStorageClient,
            IFileSystem fileSystem)
        {
            _configurationProvider = configurationProvider;
            _cloudStorageClient = cloudStorageClient;
            _fileSystem = fileSystem;
        }

        public async Task<string> DownloadFromInstrumentBucketAsync(string fileName)
        {
            var filePath = _fileSystem.Path.Combine(
                _configurationProvider.TempPath,
                "InstrumentPackages",
                Guid.NewGuid().ToString());

            return await DownloadFromBucketAsync(_configurationProvider.DqsBucket, fileName, filePath);
        }

        public async Task<string> DownloadFromBucketAsync(string bucketName, string fileName, string filePath)
        {
            if (!_fileSystem.Directory.Exists(filePath))
            {
                _fileSystem.Directory.CreateDirectory(filePath);
            }

            var destinationFilePath = _fileSystem.Path.Combine(filePath, fileName);
            await _cloudStorageClient.DownloadAsync(bucketName, fileName, destinationFilePath);

            return destinationFilePath;
        }
    }
}