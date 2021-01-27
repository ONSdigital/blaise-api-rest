using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentDataService : IInstrumentDataService
    {
        private readonly IBlaiseFileService _fileService;
        private readonly ICloudStorageService _storageService;

        public InstrumentDataService(
            IBlaiseFileService fileService,
            ICloudStorageService storageService)
        {
            _fileService = fileService;
            _storageService = storageService;
        }

        public async Task<string> DeliverInstrumentPackageWithDataAsync(string serverParkName,
            InstrumentPackageDto instrumentPackageDto)
        {
            var instrumentPackage = await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentPackageDto);

            return await UploadInstrumentToBucketAsync(instrumentPackageDto.BucketPath, instrumentPackage);
        }

        public async Task<string> DownloadInstrumentPackageWithDataAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto)
        {
            return await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentPackageDto);
        }

        private async Task<string> CreateInstrumentPackageWithDataAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentPackageDto.BucketPath.ThrowExceptionIfNullOrEmpty("instrumentPackageDto.BucketPath");
            instrumentPackageDto.InstrumentFile.ThrowExceptionIfNullOrEmpty("instrumentPackageDto.InstrumentFile");

            var instrumentPackage = await DownloadInstrumentFromBucketAsync(instrumentPackageDto);
      
            _fileService.UpdateInstrumentFileWithData(serverParkName,instrumentPackage);

            return instrumentPackage;
        }

        private async Task<string> DownloadInstrumentFromBucketAsync(InstrumentPackageDto instrumentPackageDto)
        {
            var instrumentPackage = _fileService.GenerateUniqueInstrumentFile(instrumentPackageDto.InstrumentFile);

            return await _storageService.DownloadFromBucketAsync(instrumentPackageDto.BucketPath, 
                instrumentPackageDto.InstrumentFile,instrumentPackage);
        }

        private async Task<string> UploadInstrumentToBucketAsync(string bucketPath, string instrumentFile)
        {
            var instrumentName = _fileService.GetInstrumentNameFromFile(instrumentFile);
            var dataBucketPath = $"{bucketPath}/data/{instrumentName}";
            await _storageService.UploadToBucketAsync(dataBucketPath, instrumentFile);

            _fileService.DeleteFile(instrumentFile);

            return dataBucketPath;
        }
    }
}
