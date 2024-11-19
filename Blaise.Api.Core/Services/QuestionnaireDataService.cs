using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class QuestionnaireDataService : IQuestionnaireDataService
    {
        private readonly IFileService _fileService;
        private readonly INisraFileImportService _nisraService;
        private readonly ICloudStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public QuestionnaireDataService(
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

        public async Task ImportOnlineDataAsync(QuestionnaireDataDto questionnaireDataDto, string serverParkName, string questionnaireName,
            string tempFilePath)
        {
            questionnaireDataDto.ThrowExceptionIfNull("questionnaireDataDto");
            questionnaireDataDto.QuestionnaireDataPath.ThrowExceptionIfNullOrEmpty("questionnaireDataDto.QuestionnaireDataPath");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            await DownloadDatabaseFilesFromBucketAsync(questionnaireDataDto.QuestionnaireDataPath, tempFilePath);
            var databaseFile = _fileService.GetDatabaseFile(tempFilePath, questionnaireName);

            _nisraService.ImportNisraDatabaseFile(databaseFile, questionnaireName, serverParkName);
            _fileService.RemovePathAndFiles(tempFilePath);
        }

        private async Task DownloadDatabaseFilesFromBucketAsync(string bucketPath, string tempFilePath)
        {
            _loggingService.LogInfo($"Downloading questionnaire files from nisra bucket path '{bucketPath}'");
            await _storageService.DownloadFilesFromNisraBucketAsync(bucketPath, tempFilePath);
        }
    }
}
