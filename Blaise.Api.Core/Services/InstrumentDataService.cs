using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentDataService : IInstrumentDataService
    {
        private readonly IFileService _fileService;
        private readonly INisraService _caseService;
        private readonly ICloudStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public InstrumentDataService(
            IFileService fileService,
            INisraService caseService,
            ICloudStorageService storageService, 
            ILoggingService loggingService)
        {
            _fileService = fileService;
            _caseService = caseService;
            _storageService = storageService;
            _loggingService = loggingService;
        }

        public async Task<string> GetInstrumentPackageWithDataAsync(string serverParkName, string instrumentName, string tempFilePath)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            return await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentName, tempFilePath);
        }

        public async Task ImportOnlineDataAsync(InstrumentDataDto instrumentDataDto, string serverParkName,
            string instrumentName, string tempFilePath)
        {
            instrumentDataDto.ThrowExceptionIfNull("InstrumentDataDto");
            instrumentDataDto.InstrumentDataPath.ThrowExceptionIfNullOrEmpty("instrumentDataDto.InstrumentDataPath");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            await DownloadDatabaseFilesFromBucketAsync(instrumentDataDto.InstrumentDataPath, tempFilePath);
            var databaseFile = _fileService.GetDatabaseFile(tempFilePath, instrumentName);

            _caseService.ImportOnlineDatabaseFile(databaseFile, instrumentName, serverParkName);
        }

        private async Task<string> CreateInstrumentPackageWithDataAsync(string serverParkName, string instrumentName, string tempFilePath)
        {
            var instrumentPackage = await DownloadInstrumentFromBucketAsync(instrumentName, tempFilePath);
            _loggingService.LogInfo($"Downloaded instrument package '{instrumentPackage}'");
            
            _fileService.UpdateInstrumentFileWithData(serverParkName, instrumentPackage);
            _loggingService.LogInfo($"Updated instrument package '{instrumentPackage}' with data");

            return instrumentPackage;
        }

        private async Task<string> DownloadInstrumentFromBucketAsync(string instrumentName, string tempFilePath)
        {
            var instrumentPackageName = _fileService.GetInstrumentPackageName(instrumentName);

            _loggingService.LogInfo($"Downloading instrument package '{instrumentPackageName}'");
            return await _storageService.DownloadPackageFromInstrumentBucketAsync(instrumentPackageName, tempFilePath);
        }

        private async Task DownloadDatabaseFilesFromBucketAsync(string bucketPath, string tempFilePath)
        {
            _loggingService.LogInfo($"Downloading instrument files from nisra bucket path '{bucketPath}'");
            await _storageService.DownloadDatabaseFilesFromNisraBucketAsync(bucketPath, tempFilePath);
        }
    }
}
