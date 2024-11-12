using System.Collections.Generic;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.Layout.Types;

namespace Blaise.Api.Core.Services
{
    public class IngestService : IIngestService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly IFileService _fileService;
        private readonly ICloudStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public IngestService(
            IBlaiseCaseApi blaiseApi,
            IFileService fileService,
            ICloudStorageService storageService, 
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _fileService = fileService;
            _storageService = storageService;
            _loggingService = loggingService;
        }

        public async Task IngestDataAsync(QuestionnaireDataDto questionnaireDataDto, string serverParkName, string questionnaireName,
            string tempFilePath)
        {
            questionnaireDataDto.ThrowExceptionIfNull("questionnaireDataDto");
            questionnaireDataDto.QuestionnaireDataPath.ThrowExceptionIfNullOrEmpty("questionnaireDataDto.QuestionnaireDataPath");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            tempFilePath.ThrowExceptionIfNullOrEmpty("tempFilePath");

            await DownloadDatabaseFilesFromBucketAsync(questionnaireDataDto.QuestionnaireDataPath, tempFilePath);
            var databaseFile = _fileService.GetDatabaseFile(tempFilePath, questionnaireName);

            IngestQuestionnaireData(databaseFile, questionnaireName, serverParkName);
            _fileService.RemovePathAndFiles(tempFilePath);
        }

        private void IngestQuestionnaireData(string databaseFile, string questionnaireName, string serverParkName)
        {
            var dataRecords = _blaiseApi.GetCases(databaseFile);
            var caseModels = new List<CaseModel>();

            while (!dataRecords.EndOfSet)
            {
                caseModels.Add(BuildCaseModel(dataRecords.ActiveRecord));
                dataRecords.MoveNext();

                if (caseModels.Count == 50 || dataRecords.EndOfSet)
                {
                    _blaiseApi.CreateCases(caseModels, questionnaireName, serverParkName);
                    caseModels.Clear();
                }
            }
        }

        private CaseModel BuildCaseModel(IDataRecord dataRecord)
        {
            return new CaseModel(
                _blaiseApi.GetPrimaryKeyValues(dataRecord),
                _blaiseApi.GetRecordDataFields(dataRecord));
        }

        private async Task DownloadDatabaseFilesFromBucketAsync(string bucketPath, string tempFilePath)
        {
            _loggingService.LogInfo($"Downloading questionnaire files from nisra bucket path '{bucketPath}'");
            await _storageService.DownloadDatabaseFilesFromNisraBucketAsync(bucketPath, tempFilePath);
        }
    }
}
