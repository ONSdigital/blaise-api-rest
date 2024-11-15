
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public static string IngestDatabaseFile = $"{BlaiseConfigurationHelper.QuestionnaireName}.bdix";
        public static string IngestFile = $"{BlaiseConfigurationHelper.QuestionnaireName}.zip";

        public static IngestFileHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new IngestFileHelper());
        }

        public async Task CreateCasesInIngestFileAsync(IEnumerable<CaseModel> caseModels, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, IngestDatabaseFile);

            CaseHelper.GetInstance().CreateCasesInFile(questionnaireDatabase, caseModels.ToList());

            string filePath = Path.Combine(path, IngestFile);
            extractedFilePath.ZipFiles(filePath);
            await UploadFileToBucket(filePath);
        }


        public async Task CleanUpIngestFiles()
        {
            Console.WriteLine($"IngestFileHelper - clean up file '{IngestFile}' from '{BlaiseConfigurationHelper.IngestBucket}'");
            await CloudStorageHelper.GetInstance().DeleteFileInBucketAsync(BlaiseConfigurationHelper.IngestBucket,
                IngestFile);
        }

        private string ExtractPackageFiles(string path, string questionnairePackage)
        {
            Console.WriteLine($"IngestFileHelper - extract file '{questionnairePackage}' to '{path}'");
            var extractedFilePath = Path.Combine(path, BlaiseConfigurationHelper.QuestionnaireName);

            questionnairePackage.ExtractFiles(extractedFilePath);

            return extractedFilePath;
        }

        private async Task UploadFileToBucket(string filePath)
        {
            Console.WriteLine($"IngestFileHelper - Upload file '{filePath}' to '{BlaiseConfigurationHelper.IngestBucket}'");
            await CloudStorageHelper.GetInstance().UploadFileToBucketAsync(
                BlaiseConfigurationHelper.IngestBucket, 
                filePath);
        }
    }
}
