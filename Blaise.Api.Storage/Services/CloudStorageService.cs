using System;
using System.IO.Abstractions;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Exceptions;
using Google;

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

        public async Task<string> DownloadFileFromIngestBucketAsync(string filePath, string tempFilePath)
        {
            return await DownloadFileFromBucketAsync(_configurationProvider.IngestBucket, filePath, tempFilePath);
        }

        public async Task DeleteFileFromIngestBucketAsync(string filePath)
        {
            await DeleteFileFromBucketAsync(_configurationProvider.IngestBucket, filePath);
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

            _loggingService.LogInfo($"Attempting to Download file '{bucketFilePath}' file from bucket '{bucketName}' to {downloadedFile}");

            await _cloudStorageClient.DownloadAsync(bucketName, bucketFilePath, downloadedFile);

            _loggingService.LogInfo($"Downloaded '{fileName}' from bucket '{bucketName}' to '{tempFilePath}'");

            return downloadedFile;
        }



        public async Task DeleteFileFromBucketAsync(string bucketName, string bucketFilePath)
        {
            var fileName = _fileSystem.Path.GetFileName(bucketFilePath);

            _loggingService.LogInfo($"Attempting to Delete file '{bucketFilePath}' file from bucket '{bucketName}'");

            try
            {
                await _cloudStorageClient.DeleteAsync(bucketName, bucketFilePath);
                _loggingService.LogInfo($"Deleted '{fileName}' from bucket '{bucketName}'");
            }
            catch (GoogleApiException apiEx)
            {
                if (apiEx.Error.Code == 404)
                {
                    _loggingService.LogWarn($"'{bucketFilePath}' not found in bucket '{bucketName}'. Skipping deletion.");
                }
                else
                {
                    _loggingService.LogError($"Error deleting '{bucketName}' from bucket '{bucketName}'.", apiEx);
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Unexpected error occurred while trying to delete '{bucketFilePath}' from bucket '{bucketName}'.", ex);
            }

        }
    }
}