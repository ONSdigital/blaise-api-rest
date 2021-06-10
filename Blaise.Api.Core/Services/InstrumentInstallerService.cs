using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentInstallerService : IInstrumentInstallerService
    {
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;
        private readonly IFileService _fileService;
        private readonly ICloudStorageService _storageService;

        public InstrumentInstallerService(
            IBlaiseSurveyApi blaiseApi,
            IFileService fileService,
            ICloudStorageService storageService)
        {
            _blaiseSurveyApi = blaiseApi;
            _fileService = fileService;
            _storageService = storageService;
        }

        public async Task<string> InstallInstrumentAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto, string tempFilePath)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentPackageDto.InstrumentFile.ThrowExceptionIfNullOrEmpty("instrumentPackageDto.InstrumentFile");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            var instrumentFile = await _storageService.DownloadPackageFromInstrumentBucketAsync(instrumentPackageDto.InstrumentFile, tempFilePath);

            _fileService.UpdateInstrumentFileWithSqlConnection(instrumentFile);

            var instrumentName = _fileService.GetInstrumentNameFromFile(instrumentPackageDto.InstrumentFile);

            _blaiseSurveyApi.InstallSurvey(
                instrumentName, 
                serverParkName, 
                instrumentFile, 
                SurveyInterviewType.Cawi);

            return instrumentName;
        }
    }
}
