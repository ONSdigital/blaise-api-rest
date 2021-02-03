using System;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
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

        private Mock<IBlaiseFileService> _fileServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _instrumentFile;
        private string _instrumentName;

        [SetUp]
        public void SetUpTests()
        {
            _fileServiceMock = new Mock<IBlaiseFileService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _loggingMock = new Mock<ILoggingService>();
            _mockSequence = new MockSequence();

            _instrumentFile = "OPN2010A.zip";
            _serverParkName = "ServerParkA";
            _instrumentName = "OPN2010A";

            _sut = new InstrumentDataService(
                _fileServiceMock.Object,
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

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromInstrumentBucketAsync(It.IsAny<string>()))
                .ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .UpdateInstrumentFileWithData(It.IsAny<string>(), It.IsAny<string>()));

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromInstrumentBucketAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(""));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(It.IsAny<string>()));

            //act
            await _sut.GetInstrumentPackageWithDataAsync(_serverParkName, _instrumentName);

            //assert
            _fileServiceMock.Verify(v => v.GetInstrumentPackageName(_instrumentName), Times.Once);

            _storageServiceMock.Verify(v => v.DownloadFromInstrumentBucketAsync(_instrumentFile), Times.Once);

            _fileServiceMock.Verify(v => v.UpdateInstrumentFileWithData(_serverParkName,
                instrumentFilePath), Times.Once);
        }


        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName, 
                string.Empty));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetInstrumentPackageWithDataAsync(_serverParkName,
               null));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.GetInstrumentPackageWithDataAsync(string.Empty, 
                _instrumentName));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DownloadInstrumentPackageWithDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetInstrumentPackageWithDataAsync(null,
                _instrumentName));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
