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
            const string BucketName = "DQS";
            const string BucketFilePath = "OPN1234/OPN1234.zip";
            const string FileName = "OPN1234.zip";
            const string TempFilePath = @"d:\temp\GUID";
            const string LocalFilePath = $@"d:\temp\GUID\{FileName}";

            _configurationProviderMock.Setup(c => c.DqsBucket).Returns(BucketName);

            _fileSystemMock.Setup(f => f.Directory.Exists(TempFilePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(BucketFilePath)).Returns(FileName);
            _fileSystemMock.Setup(s => s.Path.Combine(TempFilePath, FileName))
                .Returns(LocalFilePath);

            // act
            await _sut.DownloadFileFromQuestionnaireBucketAsync(BucketFilePath, TempFilePath);

            // assert
            _storageProviderMock.Verify(v => v.DownloadAsync(
                BucketName,
                BucketFilePath,
                LocalFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadDatabaseFilesFromNisraBucketAsync_Then_The_Correct_BucketName_Is_Provided()
        {
            // arrange
            const string BucketName = "NISRA";
            const string BucketFilePath = "OPN1234";
            const string TempFilePath = @"d:\temp\GUID";
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
                _fileSystemMock.Setup(s => s.Path.Combine(TempFilePath, file)).Returns($@"{TempFilePath}\\{file}");
            }

            _configurationProviderMock.Setup(c => c.NisraBucket).Returns(BucketName);

            _fileSystemMock.Setup(f => f.Directory.Exists(TempFilePath)).Returns(true);

            // act
            await _sut.DownloadFilesFromNisraBucketAsync(BucketFilePath, TempFilePath);

            // assert
            _storageProviderMock.Verify(
                v => v.GetListOfFiles(
                BucketName,
                BucketFilePath), Times.Once);

            foreach (var file in files)
            {
                _storageProviderMock.Verify(v => v.DownloadAsync(BucketName, file, $@"{TempFilePath}\\{file}"));
            }
        }

        [Test]
        public void Given_No_Files_Are_In_The_BucketPath_When_I_Call_DownloadDatabaseFilesFromNisraBucketAsync_Then_A_DataNotFoundException_Is_Thrown()
        {
            // arrange
            const string BucketName = "NISRA";
            const string TempPath = @"d:\Temp";
            const string BucketFilePath = "OPN1234";
            const string LocalFilePath = @"d:\temp\QuestionnaireFiles\GUID";

            _storageProviderMock.Setup(s => s.GetListOfFiles(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<string>());

            _configurationProviderMock.Setup(c => c.TempPath).Returns(TempPath);
            _configurationProviderMock.Setup(c => c.NisraBucket).Returns(BucketName);

            _fileSystemMock.Setup(s => s.Path.Combine(TempPath, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(LocalFilePath);

            _fileSystemMock.Setup(f => f.Directory.Exists(LocalFilePath)).Returns(true);

            // act and assert
            var exception = Assert.ThrowsAsync<DataNotFoundException>(async () => await _sut.DownloadFilesFromNisraBucketAsync(BucketFilePath, TempPath));
            Assert.That(exception.Message, Is.EqualTo($"No files were found for bucket path '{BucketFilePath}' in bucket '{BucketName}'"));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_Services_Are_Called()
        {
            // arrange
            const string BucketName = "OPN";
            const string BucketFilePath = "OPN1234/OPN1234.zip";
            const string FileName = "OPN1234.zip";
            const string FilePath = @"d:\temp";
            const string DestinationFilePath = $@"{FilePath}\{FileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(FilePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(BucketFilePath)).Returns(FileName);
            _fileSystemMock.Setup(s => s.Path.Combine(FilePath, FileName))
                .Returns(DestinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            await _sut.DownloadFileFromBucketAsync(BucketName, BucketFilePath, FilePath);

            // assert
            _storageProviderMock.Verify(v => v.DownloadAsync(
                BucketName,
                BucketFilePath,
                DestinationFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_FilePath_Is_Returned()
        {
            // arrange
            const string BucketName = "OPN";
            const string BucketFilePath = "OPN1234/OPN1234.zip";
            const string FileName = "OPN1234.zip";
            const string FilePath = @"d:\temp";
            const string DestinationFilePath = $@"{FilePath}\{FileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(FilePath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.GetFileName(BucketFilePath)).Returns(FileName);
            _fileSystemMock.Setup(s => s.Path.Combine(FilePath, FileName))
                .Returns(DestinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            var result = await _sut.DownloadFileFromBucketAsync(BucketName, BucketFilePath, FilePath);

            // arrange
            Assert.That(result, Is.EqualTo(DestinationFilePath));
        }

        [Test]
        public async Task Given_Temp_Path_Is_Not_There_When_I_Call_DownloadFromBucket_Then_The_Temp_Path_Is_Created()
        {
            // arrange
            const string BucketName = "OPN";
            const string FileName = "OPN1234.zip";
            const string FilePath = @"d:\temp";
            const string DestinationFilePath = $@"{FilePath}\{FileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(FilePath)).Returns(false);
            _fileSystemMock.Setup(s => s.Path.Combine(FilePath, FileName))
                .Returns(DestinationFilePath);

            _storageProviderMock.Setup(s => s.DownloadAsync(
                BucketName,
                FileName,
                FilePath));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            // act
            await _sut.DownloadFileFromBucketAsync(BucketName, FileName, FilePath);

            // arrange
            _fileSystemMock.Verify(v => v.Directory.CreateDirectory(FilePath), Times.Once);
        }
    }
}
