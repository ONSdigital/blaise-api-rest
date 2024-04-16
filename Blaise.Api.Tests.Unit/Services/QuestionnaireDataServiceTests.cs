using System;
using System.Threading.Tasks;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Storage.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class QuestionnaireDataServiceTests
    {
        private QuestionnaireDataService _sut;

        private Mock<IFileService> _fileServiceMock;
        private Mock<INisraFileImportService> _nisraServiceMock;
        private Mock<ICloudStorageService> _storageServiceMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private string _serverParkName;
        private string _questionnaireName;
        private string _bucketPath;
        private string _tempPath;

        private QuestionnaireDataDto _questionnaireDataDto;

        [SetUp]
        public void SetUpTests()
        {
            _fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);
            _nisraServiceMock = new Mock<INisraFileImportService>(MockBehavior.Strict);
            _storageServiceMock = new Mock<ICloudStorageService>(MockBehavior.Strict);
            _loggingMock = new Mock<ILoggingService>();
            _mockSequence = new MockSequence();

            _serverParkName = "ServerParkA";
            _questionnaireName = "OPN2010A";
            _bucketPath = "OPN2010A";
            _tempPath = @"c:\temp\GUID";

            _questionnaireDataDto = new QuestionnaireDataDto { QuestionnaireDataPath = _bucketPath};

            _sut = new QuestionnaireDataService(
                _fileServiceMock.Object,
                _nisraServiceMock.Object,
                _storageServiceMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public async Task Given_I_Call_ImportOnlineDataAsync_Then_The_Correct_Services_Are_Called_In_The_Correct_Order()
        {
            //arrange
            var dataBaseFilePath = $@"d:\OPN\{_questionnaireName}.bdix";

            _storageServiceMock.InSequence(_mockSequence).Setup(s =>
                s.DownloadDatabaseFilesFromNisraBucketAsync(_bucketPath, _tempPath)).Returns(Task.FromResult(0));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.GetDatabaseFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dataBaseFilePath);

            _nisraServiceMock.InSequence(_mockSequence).Setup(c => c.ImportNisraDatabaseFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f.RemovePathAndFiles(It.IsAny<string>()));

            //act
          await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName, _questionnaireName, _tempPath);

            //assert
            _storageServiceMock.Verify(v => v.DownloadDatabaseFilesFromNisraBucketAsync(_bucketPath, _tempPath), Times.Once);

            _fileServiceMock.Verify(v => v.GetDatabaseFile(_tempPath, _questionnaireName), Times.Once);

            _nisraServiceMock.Verify(v => v.ImportNisraDatabaseFile(dataBaseFilePath, _questionnaireName, _serverParkName), Times.Once);

            _fileServiceMock.InSequence(_mockSequence).Setup(f => f
                .RemovePathAndFiles(_tempPath));
        }

        [Test]
        public void Given_A_Null_QuestionnaireDataDto_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(null, _serverParkName,
                _questionnaireName, _tempPath));
            Assert.AreEqual("The argument 'questionnaireDataDto' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireDataPath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            _questionnaireDataDto.QuestionnaireDataPath = string.Empty;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
                _questionnaireName, _tempPath));
            Assert.AreEqual("A value for the argument 'questionnaireDataDto.QuestionnaireDataPath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireDataPath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _questionnaireDataDto.QuestionnaireDataPath = null;

            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
               _questionnaireName, _tempPath));
            Assert.AreEqual("questionnaireDataDto.QuestionnaireDataPath", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
                string.Empty, _tempPath));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
               null, _tempPath));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, string.Empty,
                _questionnaireName, _tempPath));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, null,
                _questionnaireName, _tempPath));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_TempFilePath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
                _questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'tempFilePath' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_TempFilePath_When_I_Call_ImportOnlineDataAsync_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ImportOnlineDataAsync(_questionnaireDataDto, _serverParkName,
                _questionnaireName, null));
            Assert.AreEqual("tempFilePath", exception.ParamName);
        }
    }
}
