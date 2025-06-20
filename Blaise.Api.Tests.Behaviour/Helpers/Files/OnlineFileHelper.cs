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
using Blaise.Api.Tests.Behaviour.Models.Enums;

namespace Blaise.Api.Tests.Behaviour.Helpers.Files
{
    public class OnlineFileHelper
    {
        private static OnlineFileHelper _currentInstance;

        public static OnlineFileHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new OnlineFileHelper());
        }

        public async Task CreateCasesInOnlineFileAsync(int numberOfCases, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.QuestionnaireName + ".bdix");

            CaseHelper.GetInstance().CreateCasesInFile(questionnaireDatabase, numberOfCases);

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task CreateCasesInOnlineFileAsync(IEnumerable<CaseModel> caseModels, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.QuestionnaireName + ".bdix");

            CaseHelper.GetInstance().CreateCasesInFile(questionnaireDatabase, caseModels.ToList());

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task<string> CreateCaseInOnlineFileAsync(int outcomeCode, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.QuestionnaireName + ".bdix");

            var caseModel = CaseHelper.GetInstance().CreateCaseModel(outcomeCode.ToString(), ModeType.Web, DateTime.Now.AddMinutes(-40));
           CaseHelper.GetInstance().CreateCaseInFile(questionnaireDatabase, caseModel);

            await UploadFilesToBucket(extractedFilePath);

            return caseModel.PrimaryKey;
        }

        public async Task CreateCaseInOnlineFileAsync(CaseModel caseModel, string path)
        {
            var questionnairePackage = BlaiseConfigurationHelper.QuestionnairePackagePath;
            var extractedFilePath = ExtractPackageFiles(path, questionnairePackage);
            var questionnaireDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.QuestionnaireName + ".bdix");

            CaseHelper.GetInstance().CreateCaseInFile(questionnaireDatabase, caseModel);

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task CleanUpOnlineFiles()
        {
            await CloudStorageHelper.GetInstance().DeleteFilesInBucketAsync(BlaiseConfigurationHelper.OnlineFileBucket,
                BlaiseConfigurationHelper.QuestionnaireName);
        }

        private string ExtractPackageFiles(string path, string questionnairePackage)
        {
            var extractedFilePath = Path.Combine(path, BlaiseConfigurationHelper.QuestionnaireName);

            questionnairePackage.ExtractFiles(extractedFilePath);

            return extractedFilePath;
        }

        private async Task UploadFilesToBucket(string filePath)
        {
            await CloudStorageHelper.GetInstance().UploadFolderToBucketAsync(
                BlaiseConfigurationHelper.OnlineFileBucket, 
                filePath);
        }
    }
}
