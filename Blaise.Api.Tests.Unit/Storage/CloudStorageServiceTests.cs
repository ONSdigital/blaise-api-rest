using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Storage.Interfaces;
using Blaise.Api.Storage.Services;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Storage
{
    public class CloudStorageServiceTests
    {
        private CloudStorageService _sut;

        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<ICloudStorageClientProvider> _storageProviderMock;
        private Mock<IFileSystem> _fileSystemMock;

        [SetUp]
        public void SetUpTests()
        {
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _storageProviderMock = new Mock<ICloudStorageClientProvider>();
            _fileSystemMock = new Mock<IFileSystem>();

            _sut = new CloudStorageService(
                _configurationProviderMock.Object,
                _storageProviderMock.Object,
                _fileSystemMock.Object);
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            const string bucketPath = "OPN";
            const string instrumentFileName = "OPN1234.zip";
            const string localFileName = "DD_OPN1234.zip";
            const string tempPath = @"d:\temp";
            var filePath = $@"{tempPath}\{Guid.NewGuid()}";

            _fileSystemMock.Setup(f => f.Directory.Exists(tempPath)).Returns(true);
            _fileSystemMock.Setup(s => s.Path.Combine(tempPath, It.IsAny<string>()))
                .Returns(filePath);

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _configurationProviderMock.Setup(c => c.BucketPath).Returns(bucketPath);
            _fileSystemMock.Setup(s => s.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(filePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromBucketAsync(instrumentFileName, localFileName);

            //assert
            _storageProviderMock.Verify(v => v.DownloadAsync(bucketPath,
                instrumentFileName, filePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_File_Is_Returned()
        {
            //arrange
            const string bucketPath = "OPN";
            const string instrumentFileName = "OPN1234.zip";
            const string localFileName = "DD_OPN1234.zip";
            const string tempPath = @"d:\temp";
            var filePath = $"{tempPath}";
            var instrumentFilePath = $@"{tempPath}\{localFileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(tempPath)).Returns(true);
            _fileSystemMock.Setup(f => f.Path.Combine(tempPath, It.IsAny<string>()))
                .Returns(filePath);
            
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, localFileName))
                .Returns(instrumentFilePath);

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _storageProviderMock.Setup(s => s.DownloadAsync(bucketPath, instrumentFileName,
                It.IsAny<string>()));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            var result = await _sut.DownloadFromBucketAsync(instrumentFileName, localFileName);

            //arrange
            Assert.AreEqual(instrumentFilePath, result);
        }

        [Test]
        public async Task Given_Temp_Path_Is_Not_There_When_I_Call_DownloadFromBucket_Then_The_Temp_Path_Is_Created()
        {
            //arrange
            const string bucketPath = "OPN";
            const string instrumentFileName = "OPN1234.zip";
            const string localFileName = "DD_OPN1234.zip";
            const string tempPath = @"d:\temp";
            var filePath = $"{tempPath}";
            var instrumentFilePath = $@"{tempPath}\{localFileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(tempPath)).Returns(false);
            _fileSystemMock.Setup(f => f.Path.Combine(tempPath, It.IsAny<string>()))
                .Returns(filePath);

            _fileSystemMock.Setup(f => f.Path.Combine(filePath, localFileName))
                .Returns(instrumentFilePath);

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _storageProviderMock.Setup(s => s.DownloadAsync(bucketPath, instrumentFileName,
                It.IsAny<string>()));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromBucketAsync(instrumentFileName, localFileName);

            //arrange
            _fileSystemMock.Verify(v => v.Directory.CreateDirectory(tempPath), Times.Once);
        }

        [Test]
        public async Task Given_I_Call_UploadToBucketAsync_Then_The_File_Is_Uploaded_To_The_Correct_Path()
        {
            //arrange
            const string filePath = @"c:\\temp\OPN2010A.bpkg";
            const string uploadPath = "data";
            const string bucketPath = "OPN";
            var expectedFullUploadPath = $@"{bucketPath}\{uploadPath}";

            _fileSystemMock.Setup(f => f.Path.Combine(bucketPath, uploadPath)).Returns(expectedFullUploadPath);


            _configurationProviderMock.Setup(c => c.BucketPath).Returns(bucketPath);
            _storageProviderMock.Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<string>()));

            //act
            await _sut.UploadToBucketAsync(uploadPath, filePath);

            //arrange
            _storageProviderMock.Verify(v => v.UploadAsync(expectedFullUploadPath, filePath));
        }
    }
}