using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blaise.Api.Tests.Helpers.Case;
using Blaise.Api.Tests.Helpers.Cloud;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Api.Tests.Helpers.Extensions;
using Blaise.Api.Tests.Models.Case;
using Blaise.Api.Tests.Models.Enums;

namespace Blaise.Api.Tests.Helpers.Files
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
            var instrumentPackage = await DownloadPackageFromBucket(path);
            var extractedFilePath = ExtractPackageFiles(path, instrumentPackage);
            var instrumentDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.InstrumentName + ".bdix");

            CaseHelper.GetInstance().CreateCasesInFile(instrumentDatabase, numberOfCases);

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task CreateCasesInOnlineFileAsync(IEnumerable<CaseModel> caseModels, string path)
        {
            var instrumentPackage = await DownloadPackageFromBucket(path);
            var extractedFilePath = ExtractPackageFiles(path, instrumentPackage);
            var instrumentDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.InstrumentName + ".bdix");

            CaseHelper.GetInstance().CreateCasesInFile(instrumentDatabase, caseModels.ToList());

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task<string> CreateCaseInOnlineFileAsync(int outcomeCode, string path)
        {
            var instrumentPackage = await DownloadPackageFromBucket(path);
            var extractedFilePath = ExtractPackageFiles(path, instrumentPackage);
            var instrumentDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.InstrumentName + ".bdix");

            var caseModel = CaseHelper.GetInstance().CreateCaseModel(outcomeCode.ToString(), ModeType.Web, DateTime.Now.AddMinutes(-40));
           CaseHelper.GetInstance().CreateCaseInFile(instrumentDatabase, caseModel);

            await UploadFilesToBucket(extractedFilePath);

            return caseModel.PrimaryKey;
        }

        public async Task CreateCaseInOnlineFileAsync(CaseModel caseModel, string path)
        {
            var instrumentPackage = await DownloadPackageFromBucket(path);
            var extractedFilePath = ExtractPackageFiles(path, instrumentPackage);
            var instrumentDatabase = Path.Combine(extractedFilePath, BlaiseConfigurationHelper.InstrumentName + ".bdix");

            CaseHelper.GetInstance().CreateCaseInFile(instrumentDatabase, caseModel);

            await UploadFilesToBucket(extractedFilePath);
        }

        public async Task CleanUpOnlineFiles()
        {
            await CloudStorageHelper.GetInstance().DeleteFilesInBucketAsync(BlaiseConfigurationHelper.OnlineFileBucket,
                BlaiseConfigurationHelper.InstrumentName);
        }

        private async Task<string> DownloadPackageFromBucket(string path)
        {
            return await CloudStorageHelper.GetInstance().DownloadFromBucketAsync(
                BlaiseConfigurationHelper.InstrumentPackageBucket,
                BlaiseConfigurationHelper.InstrumentFile, path);
        }

        private string ExtractPackageFiles(string path, string instrumentPackage)
        {
            var extractedFilePath = Path.Combine(path, BlaiseConfigurationHelper.InstrumentName);

            instrumentPackage.ExtractFiles(extractedFilePath);

            return extractedFilePath;
        }

        private async Task UploadFilesToBucket(string filePath)
        {
            var uploadPath = Path.Combine(BlaiseConfigurationHelper.OnlineFileBucket);

            await CloudStorageHelper.GetInstance().UploadFolderToBucketAsync(
                uploadPath, filePath);
        }
    }
}
