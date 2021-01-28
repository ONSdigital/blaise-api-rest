﻿using System;
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
            const string tempPath = "d:\\temp";
            var filePath = $"{tempPath}\\{Guid.NewGuid()}";

            _fileSystemMock.Setup(s => s.Directory.Exists(tempPath)).Returns(true);

            _fileSystemMock.Setup(s => s.Path.Combine(tempPath, It.IsAny<string>()))
                .Returns(filePath);

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _fileSystemMock.Setup(s => s.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(filePath);
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            await _sut.DownloadFromBucketAsync(bucketPath, instrumentFileName, localFileName);

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

            _fileSystemMock.Setup(s => s.Directory.Exists(tempPath)).Returns(true);

            _fileSystemMock.Setup(s => s.Directory.Exists(tempPath)).Returns(true);

            _fileSystemMock.Setup(s => s.Path.Combine(tempPath, It.IsAny<string>()))
                .Returns(filePath);
            
            _fileSystemMock.Setup(s => s.Path.Combine(filePath, localFileName))
                .Returns(instrumentFilePath);

            _configurationProviderMock.Setup(c => c.TempPath).Returns(tempPath);
            _storageProviderMock.Setup(s => s.DownloadAsync(bucketPath, instrumentFileName,
                It.IsAny<string>()));
            _fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            //act
            var result = await _sut.DownloadFromBucketAsync(bucketPath, instrumentFileName, localFileName);

            //arrange
            Assert.AreEqual(instrumentFilePath, result);
        }
    }
}
