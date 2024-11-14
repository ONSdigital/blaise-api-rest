using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Ingest;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Api.Tests.Unit.Helpers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class IngestServiceTests
    {
        private IngestService _sut;

        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _questionnaireName;
        private string _bucketPath;
        private string _tempPath;


        private IngestDataDto _ingestDataDto;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseCaseApi>();
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _loggingMock = new Mock<ILoggingService>();
            _mockSequence = new MockSequence();

            _serverParkName = "ServerParkA";
            _questionnaireName = "OPN2010A";
            _bucketPath = "OPN2010A.bdix";
            _tempPath = @"c:\temp\GUID";

            _ingestDataDto = new IngestDataDto(_bucketPath);

            _sut = new IngestService(
                _blaiseApiMock.Object,
                _fileServiceMock.Object,
                _storageServiceMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public async Task Given_I_Call_IngestDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            var databaseFile = $@"d:\OPN\{_questionnaireName}.bdix";
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys("90001");
            var fieldData = primaryKeyValues;
            var dataRecordMock = new Mock<IDataRecord>();
            var dataSetMock = new Mock<IDataSet>();
            dataSetMock.Setup(d => d.ActiveRecord).Returns(dataRecordMock.Object);
            dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(false)
                .Returns(false)
                .Returns(true)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetCases(databaseFile)).Returns(dataSetMock.Object);

            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValues(dataRecordMock.Object)).Returns(primaryKeyValues);
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(dataRecordMock.Object)).Returns(fieldData);

            _storageServiceMock.InSequence(_mockSequence).Setup(s =>
                s.DownloadFileFromIngestBucketAsync( _bucketPath, _tempPath)).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.UnzipFile(It.IsAny<string>(), It.IsAny<string>()));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetDatabaseFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(databaseFile);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.RemovePathAndFiles(It.IsAny<string>()));



            //act
            await _sut.IngestDataAsync(_ingestDataDto, _serverParkName, _questionnaireName, _tempPath);

            //assert
            _storageServiceMock.Verify(v => v.DownloadFileFromIngestBucketAsync( _bucketPath, _tempPath), Times.Once);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UnzipFile(_bucketPath, _tempPath));

            _fileServiceMock.Verify(v => v.GetDatabaseFile(_tempPath, _questionnaireName), Times.Once);

            _blaiseApiMock.Verify(b => b.GetCases(databaseFile), Times.Once);
            _blaiseApiMock.Verify(b => b.GetPrimaryKeyValues(dataRecordMock.Object), Times.Exactly(2));
            _blaiseApiMock.Verify(b => b.GetRecordDataFields(dataRecordMock.Object), Times.Exactly(2));
            _blaiseApiMock.Verify(b => b.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName), Times.Once);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .RemovePathAndFiles(_tempPath));
        }

        [Test]
        public void Given_A_Null_ingestDataDto_When_I_Call_IngestDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.IngestDataAsync(null, _serverParkName,
                _questionnaireName, _tempPath));
            Assert.AreEqual("The argument 'ingestDataDto' must be supplied", exception.ParamName);
        }


        [Test]
        public void Given_An_Empty_BucketFilePath_When_I_Call_IngestDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            _ingestDataDto.BucketFilePath = string.Empty;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
                _questionnaireName, _tempPath));
            Assert.AreEqual("A value for the argument 'ingestDataDto.BucketFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_BucketFilePath_When_I_Call_IngestDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _ingestDataDto.BucketFilePath = null;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
               _questionnaireName, _tempPath));
            Assert.AreEqual("ingestDataDto.BucketFilePath", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_IngestDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
                string.Empty, _tempPath));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_IngestDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
               null, _tempPath));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_IngestDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.IngestDataAsync(_ingestDataDto, string.Empty,
                _questionnaireName, _tempPath));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_IngestDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.IngestDataAsync(_ingestDataDto, null,
                _questionnaireName, _tempPath));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_TempFilePath_When_I_Call_IngestDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
                _questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'tempFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TempFilePath_When_I_Call_IngestDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.IngestDataAsync(_ingestDataDto, _serverParkName,
                _questionnaireName, null));
            Assert.AreEqual("tempFilePath", exception.ParamName);
        }
    }
}
