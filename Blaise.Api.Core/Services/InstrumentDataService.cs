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
        private readonly INisraFileImportService _nisraService;
        private readonly ICloudStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public InstrumentDataService(
            IFileService fileService,
            INisraFileImportService caseService,
            ICloudStorageService storageService, 
            ILoggingService loggingService)
        {
            _fileService = fileService;
            _nisraService = caseService;
            _storageService = storageService;
            _loggingService = loggingService;
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

            _nisraService.ImportNisraDatabaseFile(databaseFile, instrumentName, serverParkName);
            _fileService.RemovePathAndFiles(tempFilePath);
        }

        private async Task DownloadDatabaseFilesFromBucketAsync(string bucketPath, string tempFilePath)
        {
            _loggingService.LogInfo($"Downloading instrument files from nisra bucket path '{bucketPath}'");
            await _storageService.DownloadDatabaseFilesFromNisraBucketAsync(bucketPath, tempFilePath);
        }
    }
}
