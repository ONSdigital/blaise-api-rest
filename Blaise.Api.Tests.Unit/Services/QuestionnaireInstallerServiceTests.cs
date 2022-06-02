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
    public class QuestionnaireInstallerServiceTests
    {
        private QuestionnaireInstallerService _sut;

        private Mock<IBlaiseQuestionnaireApi> _blaiseQuestionnaireApiMock;
        private Mock<IFileService> _fileServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _questionnaireName;
        private string _questionnaireFile;
        private string _tempPath;

        private InstrumentPackageDto _instrumentPackageDto;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseQuestionnaireApiMock = new Mock<IBlaiseQuestionnaireApi>(MockBehavior.Strict);
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _questionnaireFile = "OPN2010A.zip";
            _serverParkName = "ServerParkA";
            _questionnaireName = "OPN2010A";
            _tempPath = @"c:\temp\GUID";

            _instrumentPackageDto = new InstrumentPackageDto
            {
                InstrumentFile = _questionnaireFile
            };

            _sut = new QuestionnaireInstallerService(
                _blaiseQuestionnaireApiMock.Object,
                _fileServiceMock.Object,
                _storageServiceMock.Object);
        }

        [Test]
        public async Task Given_I_Call_InstallInstrument_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            const string instrumentFilePath = "d:\\temp\\OPN1234.zip";

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadPackageFromQuestionnaireBucketAsync(
                    _questionnaireFile, _tempPath)).ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(b => b
                .UpdateQuestionnaireFileWithSqlConnection(instrumentFilePath));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .GetQuestionnaireNameFromFile(_questionnaireFile)).Returns(_questionnaireName);

            _blaiseQuestionnaireApiMock.InSequence(_mockSequence).Setup(b => b
                .InstallQuestionnaire(_questionnaireName,_serverParkName, instrumentFilePath, QuestionnaireInterviewType.Cati));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .RemovePathAndFiles(_tempPath));

            //act
            await _sut.InstallInstrumentAsync(_serverParkName, _instrumentPackageDto, _tempPath);

            //assert
            _storageServiceMock.Verify(v => v.DownloadPackageFromQuestionnaireBucketAsync( _questionnaireFile, _tempPath), Times.Once);
            _fileServiceMock.Verify(v => v.UpdateQuestionnaireFileWithSqlConnection(instrumentFilePath), Times.Once);
            _fileServiceMock.Verify(v => v.GetQuestionnaireNameFromFile(_questionnaireFile), Times.Once);
            _blaiseQuestionnaireApiMock.Verify(v => v.InstallQuestionnaire(_questionnaireName, _serverParkName,
                instrumentFilePath, QuestionnaireInterviewType.Cati), Times.Once);
        }

        [Test]
        public async Task Given_I_Call_InstallInstrument_Then_The_The_Correct_Instrument_Name_Is_Returned()
        {
            //arrange
            const string instrumentFilePath = "d:\\temp\\OPN1234.zip";

            _storageServiceMock.InSequence(_mockSequence).Setup(s => s.DownloadPackageFromQuestionnaireBucketAsync(
                    _questionnaireFile, _tempPath)).ReturnsAsync(instrumentFilePath);

            _fileServiceMock.InSequence(_mockSequence).Setup(b => b
                .UpdateQuestionnaireFileWithSqlConnection(instrumentFilePath));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .GetQuestionnaireNameFromFile(_questionnaireFile)).Returns(_questionnaireName);

            _blaiseQuestionnaireApiMock.InSequence(_mockSequence).Setup(b => b
                .InstallQuestionnaire(_questionnaireName,_serverParkName, instrumentFilePath, QuestionnaireInterviewType.Cati));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .RemovePathAndFiles(_tempPath));

            //act
            var result = await _sut.InstallInstrumentAsync(_serverParkName, _instrumentPackageDto, _tempPath);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(_questionnaireName, result);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_InstallInstrument_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.InstallInstrumentAsync(string.Empty, 
                _instrumentPackageDto, _tempPath));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_InstallInstrument_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.InstallInstrumentAsync(null,
                _instrumentPackageDto, _tempPath));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_TempFilePath_When_I_Call_InstallInstrument_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.InstallInstrumentAsync(_serverParkName, 
                _instrumentPackageDto, string.Empty));
            Assert.AreEqual("A value for the argument 'tempFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TempFilePath_When_I_Call_InstallInstrument_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.InstallInstrumentAsync(_serverParkName,
                _instrumentPackageDto, null));
            Assert.AreEqual("tempFilePath", exception.ParamName);
        }
    }
}
