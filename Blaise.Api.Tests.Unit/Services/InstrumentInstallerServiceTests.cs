using System;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Storage.Interfaces;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class InstrumentInstallerServiceTests
    {
        private InstrumentInstallerService _sut;

        private Mock<IBlaiseSurveyApi> _blaiseSurveyApiMock;
        private Mock<IBlaiseFileService> _fileServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _instrumentName;
        private string _instrumentFile;

        private InstrumentPackageDto _instrumentPackageDto;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseSurveyApiMock = new Mock<IBlaiseSurveyApi>(MockBehavior.Strict);
            _fileServiceMock = new Mock<IBlaiseFileService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _instrumentFile = "OPN2010A.zip";
            _serverParkName = "ServerParkA";
            _instrumentName = "OPN2010A";

            _instrumentPackageDto = new InstrumentPackageDto
            {
                InstrumentFile = _instrumentFile
            };

            _sut = new InstrumentInstallerService(
                _blaiseSurveyApiMock.Object,
                _fileServiceMock.Object,
                _storageServiceMock.Object);
        }

        [Test]
        public async Task Given_I_Call_InstallInstrument_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            const string instrumentFilePath = "d:\\temp\\OPN1234.zip";

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromInstrumentBucketAsync(
                    _instrumentFile)).ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(b => b
                .UpdateInstrumentFileWithSqlConnection(instrumentFilePath));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .GetInstrumentNameFromFile(_instrumentFile)).Returns(_instrumentName);

            _blaiseSurveyApiMock.InSequence(_mockSequence).Setup(b => b
                .InstallSurvey(_instrumentName,_serverParkName, instrumentFilePath, SurveyInterviewType.Cati));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(instrumentFilePath));

            //act
            await _sut.InstallInstrumentAsync(_serverParkName, _instrumentPackageDto);

            //assert
            _storageServiceMock.Verify(v => v.DownloadFromInstrumentBucketAsync( _instrumentFile), Times.Once);
            _fileServiceMock.Verify(v => v.UpdateInstrumentFileWithSqlConnection(instrumentFilePath), Times.Once);
            _fileServiceMock.Verify(v => v.GetInstrumentNameFromFile(_instrumentFile), Times.Once);
            _blaiseSurveyApiMock.Verify(v => v.InstallSurvey(_instrumentName, _serverParkName,
                instrumentFilePath, SurveyInterviewType.Cati), Times.Once);
            _fileServiceMock.Verify(v => v.DeleteFile(instrumentFilePath), Times.Once);
        }

        [Test]
        public async Task Given_I_Call_InstallInstrument_Then_The_The_Correct_Instrument_Name_Is_Returned()
        {
            //arrange
            const string instrumentFilePath = "d:\\temp\\OPN1234.zip";

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadFromInstrumentBucketAsync(
                    _instrumentFile)).ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(b => b
                .UpdateInstrumentFileWithSqlConnection(instrumentFilePath));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .GetInstrumentNameFromFile(_instrumentFile)).Returns(_instrumentName);

            _blaiseSurveyApiMock.InSequence(_mockSequence).Setup(b => b
                .InstallSurvey(_instrumentName,_serverParkName, instrumentFilePath, SurveyInterviewType.Cati));

            _fileServiceMock.InSequence(_mockSequence).Setup(s => s.DeleteFile(instrumentFilePath));

            //act
            var result = await _sut.InstallInstrumentAsync(_serverParkName, _instrumentPackageDto);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(_instrumentName, result);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_InstallInstrument_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.InstallInstrumentAsync(string.Empty, 
                _instrumentPackageDto));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_InstallInstrument_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.InstallInstrumentAsync(null,
                _instrumentPackageDto));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
