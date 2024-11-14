
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

        public static IngestFileHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new IngestFileHelper());
        }

        public async Task CreateCasesInIngestFileAsync(IEnumerable<CaseModel> caseModels, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, $"{BlaiseConfigurationHelper.QuestionnaireName}.bdix");

            CaseHelper.GetInstance().CreateCasesInFile(questionnaireDatabase, caseModels.ToList());

            string filePath = Path.Combine(path, $"{BlaiseConfigurationHelper.QuestionnaireName}.zip");
            extractedFilePath.ZipFiles(filePath);
            await UploadFileToBucket(filePath);
        }


        public async Task CleanUpIngestFiles()
        {
            await CloudStorageHelper.GetInstance().DeleteFileInBucketAsync(BlaiseConfigurationHelper.IngestBucket,
                $"{BlaiseConfigurationHelper.QuestionnaireName}.zip");
        }

        private string ExtractPackageFiles(string path, string questionnairePackage)
        {
            var extractedFilePath = Path.Combine(path, BlaiseConfigurationHelper.QuestionnaireName);

            questionnairePackage.ExtractFiles(extractedFilePath);

            return extractedFilePath;
        }

        private async Task UploadFileToBucket(string filePath)
        {
            await CloudStorageHelper.GetInstance().UploadFileToBucketAsync(
                BlaiseConfigurationHelper.IngestBucket, 
                filePath);
        }
    }
}
