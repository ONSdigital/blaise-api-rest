using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Extensions;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

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

        public async Task<string> InstallQuestionnaireAsync(string serverParkName, QuestionnairePackageDto questionnairePackageDto,
            string tempFilePath)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnairePackageDto.QuestionnaireFile.ThrowExceptionIfNullOrEmpty("QuestionnairePackageDto.ThrowExceptionIfNullOrEmpty");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            var questionnaireFile = await _storageService.DownloadPackageFromQuestionnaireBucketAsync(questionnairePackageDto.QuestionnaireFile, tempFilePath);

            _fileService.UpdateQuestionnaireFileWithSqlConnection(questionnaireFile);

            var questionnaireName = _fileService.GetQuestionnaireNameFromFile(questionnairePackageDto.QuestionnaireFile);

            var installOptions = new InstallOptions
            {
                DataEntrySettingsName = QuestionnaireDataEntryType.StrictInterviewing.ToString(),
                InitialAppLayoutSetGroupName = QuestionnaireInterviewType.Cati.FullName(),
                LayoutSetGroupName = QuestionnaireInterviewType.Cati.FullName(),
                OverwriteMode = DataOverwriteMode.Always,
            };

            _blaiseQuestionnaireApi.InstallQuestionnaire(
                questionnaireName,
                serverParkName,
                questionnaireFile,
                installOptions);

            _fileService.RemovePathAndFiles(tempFilePath);

            return questionnaireName;
        }
    }
}
