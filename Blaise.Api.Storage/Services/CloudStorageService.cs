using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Exceptions;

namespace Blaise.Api.Storage.Services
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ICloudStorageClientProvider _cloudStorageClient;
        private readonly IFileSystem _fileSystem;
        private readonly ILoggingService _loggingService;

        public CloudStorageService(
            IConfigurationProvider configurationProvider,
            ICloudStorageClientProvider cloudStorageClient,
            IFileSystem fileSystem, 
            ILoggingService loggingService)
        {
            _configurationProvider = configurationProvider;
            _cloudStorageClient = cloudStorageClient;
            _fileSystem = fileSystem;
            _loggingService = loggingService;
        }

        public async Task<string> DownloadPackageFromQuestionnaireBucketAsync(string fileName, string tempFilePath)
        {

            _loggingService.LogInfo($"Attempting to download package '{fileName}' from bucket '{_configurationProvider.DqsBucket}'");

            return await DownloadFromBucketAsync(_configurationProvider.DqsBucket, fileName, tempFilePath);
        }

        public async Task DownloadDatabaseFilesFromNisraBucketAsync(string bucketPath, string tempFilePath)
        {
            var bucketFiles = (await _cloudStorageClient.GetListOfFiles(_configurationProvider.NisraBucket, bucketPath)).ToList();

            if (!bucketFiles.Any())
            {
                throw new DataNotFoundException($"No files were found for bucket path '{bucketPath}' in bucket '{_configurationProvider.NisraBucket}'");
            }

            _loggingService.LogInfo($"Attempting to Download '{bucketFiles.Count}' files from bucket '{_configurationProvider.NisraBucket}'");

            foreach (var bucketFile in bucketFiles)
            {
                await DownloadFromBucketAsync(_configurationProvider.NisraBucket, bucketFile, tempFilePath);
            }

            _loggingService.LogInfo($"Downloaded '{bucketFiles.Count}' files from bucket '{_configurationProvider.NisraBucket}'");
        }

        public async Task<string> DownloadFromBucketAsync(string bucketName, string bucketFilePath, string tempFilePath)
        {
            if (!_fileSystem.Directory.Exists(tempFilePath))
            {
                _fileSystem.Directory.CreateDirectory(tempFilePath);
            }

            var fileName = _fileSystem.Path.GetFileName(bucketFilePath);
            var downloadedFile = _fileSystem.Path.Combine(tempFilePath, fileName);

            await _cloudStorageClient.DownloadAsync(bucketName, bucketFilePath, downloadedFile);

            _loggingService.LogInfo($"Downloaded '{fileName}' from bucket '{bucketName}' to '{tempFilePath}'");

            return downloadedFile;
        }
    }
}