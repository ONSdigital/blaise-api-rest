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

        public async Task<string> DeliverInstrumentPackageWithDataAsync(string serverParkName, string instrumentName, 
            DeliverInstrumentDto deliverInstrumentDto)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            deliverInstrumentDto.BucketPath.ThrowExceptionIfNullOrEmpty("deliverInstrumentDto.BucketPath");

            var instrumentPackage = await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentName);

            return await UploadInstrumentToBucketAsync(deliverInstrumentDto.BucketPath, instrumentName, instrumentPackage);
        }

        public async Task<string> DownloadInstrumentPackageWithDataAsync(string serverParkName, string instrumentName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");

            return await CreateInstrumentPackageWithDataAsync(serverParkName, instrumentName);
        }

        private async Task<string> CreateInstrumentPackageWithDataAsync(string serverParkName, string instrumentName)
        {
            var instrumentPackage = await DownloadInstrumentFromBucketAsync(instrumentName);
      
            _fileService.UpdateInstrumentFileWithData(serverParkName,instrumentPackage);

            return instrumentPackage;
        }

        private async Task<string> UploadInstrumentToBucketAsync(string bucketPath, string instrumentNameName, string instrumentFile)
        {
            var dataBucketPath = $"{bucketPath}/data/{instrumentNameName}";
            await _storageService.UploadToBucketAsync(dataBucketPath, instrumentFile);

            _fileService.DeleteFile(instrumentFile);

            return dataBucketPath;
        }

        private async Task<string> DownloadInstrumentFromBucketAsync(string instrumentName)
        {
            var instrumentPackageName = _fileService.GetInstrumentPackageName(instrumentName);
            var deliveryFileName = _fileService.GenerateUniqueInstrumentFile(instrumentPackageName);

            return await _storageService.DownloadFromBucketAsync(instrumentPackageName,deliveryFileName);
        }
    }
}
