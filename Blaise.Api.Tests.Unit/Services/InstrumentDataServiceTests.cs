using System;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Storage.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class InstrumentDataServiceTests
    {
        private InstrumentDataService _sut;

        private Mock<IFileService> _fileServiceMock;
        private Mock<INisraFileImportService> _nisraServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _instrumentFile;
        private string _instrumentName;
        private string _bucketPath;
        private string _tempPath;

        private InstrumentDataDto _instrumentDataDto;

        [SetUp]
        public void SetUpTests()
        {
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            _nisraServiceMock = new Mock<INisraFileImportService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _loggingMock = new Mock<ILoggingService>();
            _mockSequence = new MockSequence();

            _instrumentFile = "OPN2010A.zip";
            _serverParkName = "ServerParkA";
            _instrumentName = "OPN2010A";
            _bucketPath = "OPN2010A";
            _tempPath = @"c:\temp\GUID";

            _instrumentDataDto = new InstrumentDataDto {InstrumentDataPath = _bucketPath};

            _sut = new InstrumentDataService(
                _fileServiceMock.Object,
                _nisraServiceMock.Object,
                _storageServiceMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public async Task Given_I_Call_DownloadInstrumentPackageWithDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            const string instrumentFilePath = @"d:\temp\OPN2004A.zip";

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetInstrumentPackageName(It.IsAny<string>()))
                .Returns(_instrumentFile);

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadPackageFromInstrumentBucketAsync(It.IsAny<string>(), _tempPath))
                .ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UpdateInstrumentFileWithData(It.IsAny<string>(), It.IsAny<string>()));

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadPackageFromInstrumentBucketAsync(It.IsAny<string>(), _tempPath))
                .Returns(Task.FromResult(""));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(It.IsAny<string>()));

            //act
            await _sut.GetInstrumentPackageWithDataAsync(_serverParkName, _instrumentName, _tempPath);

            //assert
            _fileServiceMock.Verify(v => v.GetInstrumentPackageName(_instrumentName), Times.Once);

            _storageServiceMock.Verify(v => v.DownloadPackageFromInstrumentBucketAsync(_instrumentFile, _tempPath), Times.Once);

            _fileServiceMock.Verify(v => v.UpdateInstrumentFileWithData(_serverParkName,
                instrumentFilePath), Times.Once);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName,
                string.Empty, _tempPath));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName,
               null, _tempPath));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetInstrumentPackageWithDataAsync(string.Empty,
                _instrumentName, _tempPath));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetInstrumentPackageWithDataAsync(null,
                _instrumentName, _tempPath));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_TempFilePath_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName,
                _instrumentName, string.Empty));
            Assert.AreEqual("A value for the argument 'tempFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TempFilePath_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName,
                _instrumentName, null));
            Assert.AreEqual("tempFilePath", exception.ParamName);
        }

        [Test]
        public async Task Given_I_Call_ImportOnlineDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            var dataBaseFilePath = $@"d:\OPN\{_instrumentName}.bdix";

            _storageServiceMock.InSequence(_mockSequence).Setup(s =>
                s.DownloadDatabaseFilesFromNisraBucketAsync(_bucketPath, _tempPath)).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetDatabaseFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dataBaseFilePath);

            _nisraServiceMock.InSequence(_mockSequence).Setup(c => c.ImportNisraDatabaseFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));


            //act
            await _sut.ImportOnlineDataAsync(_instrumentDataDto, _serverParkName, _instrumentName, _tempPath);

            //assert
            _storageServiceMock.Verify(v => v.DownloadDatabaseFilesFromNisraBucketAsync(_bucketPath, _tempPath), Times.Once);

            _fileServiceMock.Verify(v => v.GetDatabaseFile(_tempPath, _instrumentName), Times.Once);

            _nisraServiceMock.Verify(v => v.ImportNisraDatabaseFile(dataBaseFilePath, _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_A_Null_InstrumentDataDto_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(null,_serverParkName,
                _instrumentName, _tempPath));
            Assert.AreEqual("The argument 'InstrumentDataDto' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentDataPath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            _instrumentDataDto.InstrumentDataPath = string.Empty;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto, _serverParkName,
                _instrumentName, _tempPath));
            Assert.AreEqual("A value for the argument 'instrumentDataDto.InstrumentDataPath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentDataPath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _instrumentDataDto.InstrumentDataPath = null;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto,_serverParkName,
               _instrumentName, _tempPath));
            Assert.AreEqual("instrumentDataDto.InstrumentDataPath", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto, _serverParkName,
                string.Empty, _tempPath));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto,_serverParkName,
               null, _tempPath));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto,string.Empty,
                _instrumentName, _tempPath));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto, null,
                _instrumentName, _tempPath));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_TempFilePath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto, _serverParkName,
                _instrumentName, string.Empty));
            Assert.AreEqual("A value for the argument 'tempFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TempFilePath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_instrumentDataDto, _serverParkName,
                _instrumentName, null));
            Assert.AreEqual("tempFilePath", exception.ParamName);
        }
    }
}
