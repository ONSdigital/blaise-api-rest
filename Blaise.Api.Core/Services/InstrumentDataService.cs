using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentDataService : IInstrumentDataService
    {
        private readonly IBlaiseFileService _fileService;
        private readonly ICloudStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public InstrumentDataService(
            IBlaiseFileService fileService,
            ICloudStorageService storageService, 
            ILoggingService loggingService)
        {
            _fileService = fileService;
            _storageService = storageService;
            _loggingService = loggingService;
        }

        public async Task<string> GetInstrumentPackageWithDataAsync(string serverParkName, string instrumentName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");

            return await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentName);
        }

        private async Task<string> CreateInstrumentPackageWithDataAsync(string serverParkName, string instrumentName)
        {
            var instrumentPackage = await DownloadInstrumentFromBucketAsync(instrumentName);
            _loggingService.LogInfo($"Downloaded instrument package '{instrumentPackage}'");
            
            _fileService.UpdateInstrumentFileWithData(serverParkName, instrumentPackage);
            _loggingService.LogInfo($"Updated instrument package '{instrumentPackage}' with data");

            return instrumentPackage;
        }

        private async Task<string> DownloadInstrumentFromBucketAsync(string instrumentName)
        {
            var instrumentPackageName = _fileService.GetInstrumentPackageName(instrumentName);

            _loggingService.LogInfo($"Downloading instrument package '{instrumentPackageName}'");
            return await _storageService.DownloadFromInstrumentBucketAsync(instrumentPackageName);
        }
    }
}
