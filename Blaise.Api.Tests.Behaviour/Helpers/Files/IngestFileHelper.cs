using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blaise.Api.Logging.Services;
using Blaise.Api.Tests.Behaviour.Helpers.Case;
using Blaise.Api.Tests.Behaviour.Helpers.Cloud;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Helpers.Extensions;
using Blaise.Api.Tests.Behaviour.Models.Case;

namespace Blaise.Api.Tests.Behaviour.Helpers.Files
{
    public class IngestFileHelper
    {
        private static IngestFileHelper _currentInstance;
        private readonly TestEventLogging _logging = new TestEventLogging();
        public static string IngestDatabaseFile = $"{BlaiseConfigurationHelper.QuestionnaireName}.bdix";
        public static string IngestFile = $"{BlaiseConfigurationHelper.QuestionnaireName}.zip";

        public static IngestFileHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new IngestFileHelper());
        }

        public async Task CreateCasesInIngestFileAsync(IEnumerable<CaseModel> caseModels, string path)
        {
            _logging.LogInfo("CreateCasesInIngestFileAsync");
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, IngestDatabaseFile);

            _logging.LogInfo($"CreateCasesInIngestFileAsync - create cases in {questionnaireDatabase}");
            CaseHelper.GetInstance().CreateCasesInFile(questionnaireDatabase, caseModels.ToList());

            var filePath = Path.Combine(path, IngestFile);
            _logging.LogInfo($"CreateCasesInIngestFileAsync - ingest file = {filePath}");
            extractedFilePath.ZipFiles(filePath);

            await UploadFileToBucket(filePath);
        }

        public async Task CleanUpIngestFiles()
        {
            _logging.LogInfo($"CreateCasesInIngestFileAsync - cleanup remove file '{IngestFile}' from bucket '{BlaiseConfigurationHelper.IngestBucket}'");
            await CloudStorageHelper.GetInstance().DeleteFileInBucketAsync(
                BlaiseConfigurationHelper.IngestBucket,
                IngestFile);
        }

        private string ExtractPackageFiles(string path, string questionnairePackage)
        {
            _logging.LogInfo($"CreateCasesInIngestFileAsync - extract file '{questionnairePackage}' to '{path}'");
            var extractedFilePath = Path.Combine(path, BlaiseConfigurationHelper.QuestionnaireName);

            questionnairePackage.ExtractFiles(extractedFilePath);

            return extractedFilePath;
        }

        private async Task UploadFileToBucket(string filePath)
        {
            _logging.LogInfo($"CreateCasesInIngestFileAsync - upload file '{filePath}' to bucket '{BlaiseConfigurationHelper.IngestBucket}'");
            await CloudStorageHelper.GetInstance().UploadFileToBucketAsync(
                BlaiseConfigurationHelper.IngestBucket,
                filePath);
        }
    }
}
