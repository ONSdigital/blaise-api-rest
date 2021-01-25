﻿using System.IO.Abstractions;
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

        public async Task<string> DownloadFromBucketAsync(string bucketPath, string bucketFileName, string localFileName)
        {
            var destinationFilePath = _fileSystem.Path.Combine(_configurationProvider.TempPath, localFileName);
            await _cloudStorageClient.DownloadAsync(bucketPath, bucketFileName, destinationFilePath);

            return destinationFilePath;
        }

        public async Task UploadToBucketAsync(string bucketPath, string filePath)
        {
            await _cloudStorageClient.UploadAsync(bucketPath, filePath);
        }
    }
}