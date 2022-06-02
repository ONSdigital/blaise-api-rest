using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class QuestionnaireInstallerService : IQuestionnaireInstallerService
    {
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;
        private readonly IFileService _fileService;
        private readonly ICloudStorageService _storageService;

        public QuestionnaireInstallerService(
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi,
            IFileService fileService,
            ICloudStorageService storageService)
        {
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
            _fileService = fileService;
            _storageService = storageService;
        }

        public async Task<string> InstallInstrumentAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto, string tempFilePath)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentPackageDto.InstrumentFile.ThrowExceptionIfNullOrEmpty("instrumentPackageDto.InstrumentFile");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            var instrumentFile = await _storageService.DownloadPackageFromQuestionnaireBucketAsync(instrumentPackageDto.InstrumentFile, tempFilePath);

            _fileService.UpdateQuestionnaireFileWithSqlConnection(instrumentFile);

            var instrumentName = _fileService.GetQuestionnaireNameFromFile(instrumentPackageDto.InstrumentFile);

            _blaiseQuestionnaireApi.InstallQuestionnaire(
                instrumentName, 
                serverParkName, 
                instrumentFile, 
                QuestionnaireInterviewType.Cati);

            _fileService.RemovePathAndFiles(tempFilePath);

            return instrumentName;
        }

        public async Task<string> InstallQuestionnaireAsync(string serverParkName, QuestionnairePackageDto questionnairePackageDto,
            string tempFilePath)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnairePackageDto.QuestionnaireFile.ThrowExceptionIfNullOrEmpty("QuestionnairePackageDto.ThrowExceptionIfNullOrEmpty");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            var questionnaireFile = await _storageService.DownloadPackageFromQuestionnaireBucketAsync(questionnairePackageDto.QuestionnaireFile, tempFilePath);

            _fileService.UpdateQuestionnaireFileWithSqlConnection(questionnaireFile);

            var questionnaireName = _fileService.GetQuestionnaireNameFromFile(questionnairePackageDto.QuestionnaireFile);

            _blaiseQuestionnaireApi.InstallQuestionnaire(
                questionnaireName,
                serverParkName,
                questionnaireFile,
                QuestionnaireInterviewType.Cati);

            _fileService.RemovePathAndFiles(tempFilePath);

            return questionnaireName;
        }
    }
}
