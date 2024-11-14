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

        public async Task<string> DownloadFileFromQuestionnaireBucketAsync(string filePath, string tempFilePath)
        {

            _loggingService.LogInfo($"Attempting to download package '{filePath}' from bucket '{_configurationProvider.DqsBucket}'");

            return await DownloadFileFromBucketAsync(_configurationProvider.DqsBucket, filePath, tempFilePath);
        }

        public async Task DownloadFileFromIngestBucketAsync(string filePath, string tempFilePath)
        {
            //await DownloadFileFromBucketAsync(_configurationProvider.IngestBucket, filePath, tempFilePath);
            await DownloadFilesFromBucketAsync(_configurationProvider.IngestBucket, filePath, tempFilePath);
        }

        public async Task DownloadFilesFromNisraBucketAsync(string folderPath, string tempFilePath)
        {
            await DownloadFilesFromBucketAsync(_configurationProvider.NisraBucket, folderPath, tempFilePath);
        }

        public async Task DownloadFilesFromBucketAsync(string bucketName, string bucketPath, string tempFilePath)
        {
            var bucketFiles = (await _cloudStorageClient.GetListOfFiles(bucketName, bucketPath)).ToList();

            if (!bucketFiles.Any())
            {
                throw new DataNotFoundException($"No files were found for bucket path '{bucketPath}' in bucket '{bucketName}'");
            }

            _loggingService.LogInfo($"Attempting to Download '{bucketFiles.Count}' files '{string.Join(", ", bucketFiles)}' from bucket '{bucketName}'");

            foreach (var bucketFile in bucketFiles)
            {
                await DownloadFileFromBucketAsync(bucketName, bucketFile, tempFilePath);
            }

            _loggingService.LogInfo($"Downloaded '{bucketFiles.Count}' files from bucket '{bucketName}'");
        }
        
        public async Task<string> DownloadFileFromBucketAsync(string bucketName, string bucketFilePath, string tempFilePath)
        {
            if (!_fileSystem.Directory.Exists(tempFilePath))
            {
                _fileSystem.Directory.CreateDirectory(tempFilePath);
            }

            var fileName = _fileSystem.Path.GetFileName(bucketFilePath);
            var downloadedFile = _fileSystem.Path.Combine(tempFilePath, fileName);

            _loggingService.LogInfo($"Attempting to Download '{downloadedFile}' file from '{bucketFilePath}'");

            await _cloudStorageClient.DownloadAsync(bucketName, bucketFilePath, downloadedFile);

            _loggingService.LogInfo($"Downloaded '{fileName}' from bucket '{bucketName}' to '{tempFilePath}'");

            return downloadedFile;
        }
    }
}