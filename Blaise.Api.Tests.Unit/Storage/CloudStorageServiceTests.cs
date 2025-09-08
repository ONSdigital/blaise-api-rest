namespace Blaise.Api.Tests.Unit.Storage
{
    using System.Collections.Generic;
    using System.IO.Abstractions;
    using System.Threading.Tasks;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Storage.Interfaces;
    using Blaise.Api.Storage.Services;
    using Blaise.Nuget.Api.Contracts.Exceptions;
    using Moq;
    using NUnit.Framework;

    public class CloudStorageServiceTests
    {
        private CloudStorageService _sut;

        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<ICloudStorageClientProvider> _storageProviderMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ILoggingService> _loggingMock;

        [SetUp]
        public void SetUpTests()
        {
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _storageProviderMock = new Mock<ICloudStorageClientProvider>();
            _fileSystemMock = new Mock<IFileSystem>();
            _loggingMock = new Mock<ILoggingService>();

            _sut = new CloudStorageService(
                _configurationProviderMock.Object,
                _storageProviderMock.Object,
                _fileSystemMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public async Task Given_I_Call_DownloadPackageFromQuestionnaireBucketAsync_Then_The_Correct_File_Is_Downloaded()
        {
            // arrange
            const string bucketName = "DQS";
            const string bucketFilePath = "OPN1234/OPN1234.zip";
            const string fileName = "OPN1234.zip";
            var tempFilePath = @"d:\temp\GUID";
            var localFilePath = @"d:\temp\GUID\OPN1234.zip";

            _configurationProviderMock.Setup(c => c.DqsBucket).Returns(bucketName);

            _fileSystemMock.Setup(f => f.Directory.Exists(tempFilePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(bucketFilePath)).Returns(fileName);
            _fileSystemMock.Setup(s => s.Path.Combine(tempFilePath, fileName))
                .Returns(localFilePath);

            // act
            await _sut.DownloadFileFromQuestionnaireBucketAsync(bucketFilePath, tempFilePath);

            // assert
            _storageProviderMock.Verify(v => v.DownloadAsync(
                bucketName,
                bucketFilePath,
                localFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadDatabaseFilesFromNisraBucketAsync_Then_The_Correct_BucketName_Is_Provided()
        {
            // arrange
            const string bucketName = "NISRA";
            const string bucketFilePath = "OPN1234";
            var tempFilePath = @"d:\temp\GUID";
            var files = new List<string>()
            {
                "OPN.bdix",
                "OPN.blix",
                "OPN.bmix",
            };

            _storageProviderMock.Setup(s => s.GetListOfFiles(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(files);

            foreach (var file in files)
            {
                _fileSystemMock.Setup(f => f.Path.GetFileName(file)).Returns(file);
                _fileSystemMock.Setup(s => s.Path.Combine(tempFilePath, file)).Returns($@"{tempFilePath}\\{file}");
            }

            _configurationProviderMock.Setup(c => c.NisraBucket).Returns(bucketName);

            _fileSystemMock.Setup(f => f.Directory.Exists(tempFilePath)).Returns(true);

            // act
            await _sut.DownloadFilesFromNisraBucketAsync(bucketFilePath, tempFilePath);

            // assert
            _storageProviderMock.Verify(
                v => v.GetListOfFiles(
                bucketName,
                bucketFilePath), Times.Once);

            foreach (var file in files)
            {
                _storageProviderMock.Verify(v => v.DownloadAsync(bucketName, file, $@"{tempFilePath}\\{file}"));
            }
        }

        [Test]
        public void Given_No_Files_Are_In_The_BucketPath_When_I_Call_DownloadDatabaseFilesFromNisraBucketAsync_Then_A_DataNotFoundException_Is_Thrown()
        {
            // arrange
            const string bucketName = "NISRA";
            const string tempPath = @"d:\Temp";
            const string bucketFilePath = "OPN1234";
            var localFilePath = @"d:\temp\QuestionnaireFiles\GUID";

            _storageProviderMock.Setup(s => s.GetListOfFiles(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string>());

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _configurationProviderMock.Setup(c => c.NisraBucket).Returns(bucketName);

            _fileSystemMock.Setup(s => s.Path.Combine(tempPath, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(localFilePath);

            _fileSystemMock.Setup(f => f.Directory.Exists(localFilePath)).Returns(true);

            // act and assert
            var exception = Assert.ThrowsAsync<DataNotFoundException>(async () => await _sut.DownloadFilesFromNisraBucketAsync(bucketFilePath, tempPath));
            Assert.AreEqual($"No files were found for bucket path '{bucketFilePath}' in bucket '{bucketName}'", exception.Message);
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_Services_Are_Called()
        {
            // arrange
            const string bucketName = "OPN";
            const string bucketFilePath = "OPN1234/OPN1234.zip";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(bucketFilePath)).Returns(fileName);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            await _sut.DownloadFileFromBucketAsync(bucketName, bucketFilePath, filePath);

            // assert
            _storageProviderMock.Verify(v => v.DownloadAsync(
                bucketName,
                bucketFilePath,
                destinationFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_FilePath_Is_Returned()
        {
            // arrange
            const string bucketName = "OPN";
            const string bucketFilePath = "OPN1234/OPN1234.zip";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(bucketFilePath)).Returns(fileName);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            var result = await _sut.DownloadFileFromBucketAsync(bucketName, bucketFilePath, filePath);

            // arrange
            Assert.AreEqual(destinationFilePath, result);
        }

        [Test]
        public async Task Given_Temp_Path_Is_Not_There_When_I_Call_DownloadFromBucket_Then_The_Temp_Path_Is_Created()
        {
            // arrange
            const string bucketName = "OPN";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(false);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);

            _storageProviderMock.Setup(s => s.DownloadAsync(
                bucketName,
                fileName,
                filePath));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            await _sut.DownloadFileFromBucketAsync(bucketName, fileName, filePath);

            // arrange
            _fileSystemMock.Verify(v => v.Directory.CreateDirectory(filePath), Times.Once);
        }
    }
}
