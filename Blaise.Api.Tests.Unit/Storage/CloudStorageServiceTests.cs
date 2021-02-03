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
        public async Task Given_I_Call_DownloadFromInstrumentBucket_Then_The_Correct_BucketName_Is_Provided()
        {
            //arrange
            const string bucketName = "OPN";
            const string tempPath = @"d:\Temp";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _configurationProviderMock.Setup(c => c.DqsBucket).Returns(bucketName);

            _fileSystemMock.Setup(s => s.Path.Combine(tempPath, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(filePath);

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(true);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromInstrumentBucketAsync(fileName);

            //assert
            _storageProviderMock.Verify(v => v.DownloadAsync(bucketName,
                fileName, destinationFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            const string bucketName = "OPN";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(true);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromBucketAsync(bucketName, fileName, filePath);

            //assert
            _storageProviderMock.Verify(v => v.DownloadAsync(bucketName,
                fileName, destinationFilePath));
        }

        [Test]
        public async Task Given_I_Call_DownloadFromBucket_Then_The_Correct_FilePath_Is_Returned()
        {
            //arrange
            const string bucketName = "OPN";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(true);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            var result = await _sut.DownloadFromBucketAsync(bucketName, fileName, filePath);

            //arrange
            Assert.AreEqual(destinationFilePath, result);
        }

        [Test]
        public async Task Given_Temp_Path_Is_Not_There_When_I_Call_DownloadFromBucket_Then_The_Temp_Path_Is_Created()
        {
            //arrange
            const string bucketName = "OPN";
            const string fileName = "OPN1234.zip";
            const string filePath = @"d:\temp";
            var destinationFilePath = $@"{filePath}\{fileName}";

            _fileSystemMock.Setup(f => f.Directory.Exists(filePath)).Returns(false);
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, fileName))
                .Returns(destinationFilePath);

            _storageProviderMock.Setup(s => s.DownloadAsync(bucketName, fileName,
                filePath));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromBucketAsync(bucketName, fileName, filePath);

            //arrange
            _fileSystemMock.Verify(v => v.Directory.CreateDirectory(filePath), Times.Once);
        }
    }
}