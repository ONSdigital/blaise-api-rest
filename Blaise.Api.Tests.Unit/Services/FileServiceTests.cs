using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class FileServiceTests
    {
        private FileService _sut;

        private Mock<IBlaiseFileApi> _blaiseFileApiMock;

        private IFileSystem _fileSystemMock;

        private string _questionnaireName;
        private string _questionnaireFile;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseFileApiMock = new Mock<IBlaiseFileApi>();

            _fileSystemMock = new MockFileSystem();

            _questionnaireFile = "OPN2010A.zip";
            _questionnaireName = "OPN2010A";

            _sut = new FileService(_blaiseFileApiMock.Object, _fileSystemMock);
        }

        [Test]
        public void Given_I_Call_UpdateQuestionnaireFileWithSqlConnection_Then_The_Correct_Services_Are_Called()
        {
            // arrange
            _blaiseFileApiMock.Setup(b => b.UpdateQuestionnaireFileWithSqlConnection(
                _questionnaireName, _questionnaireFile, true));

            // act
            _sut.UpdateQuestionnaireFileWithSqlConnection(_questionnaireFile);

            // assert
            _blaiseFileApiMock.Verify(
                v => v.UpdateQuestionnaireFileWithSqlConnection(
                _questionnaireName,
                _questionnaireFile, true), Times.Once);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireFile_When_I_Call_UpdateQuestionnaireFileWithSqlConnection_Then_An_ArgumentException_Is_Thrown()
        {
            // act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateQuestionnaireFileWithSqlConnection(string.Empty));
            Assert.AreEqual("A value for the argument 'questionnaireFile' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireFile_When_I_Call_UpdateQuestionnaireFileWithSqlConnection_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateQuestionnaireFileWithSqlConnection(null));
            Assert.AreEqual("questionnaireFile", exception.ParamName);
        }

        [Test]
        public void Given_I_Call_GetQuestionnaireNameFromFile_Then_The_Correct_Name_Is_Returned()
        {
            // act
            var result = _sut.GetQuestionnaireNameFromFile(_questionnaireFile);

            // assert
            Assert.AreEqual(_questionnaireName, result);
        }

        [Test]
        public void Given_I_Call_GetDatabaseFile_Then_The_Correct_Name_Is_Returned()
        {
            // arrange
            var filePath = @"d:\test";
            var expectedName = $@"{filePath}\{_questionnaireName}.bdix";

            // act
            var result = _sut.GetDatabaseFile(filePath, _questionnaireName);

            // assert
            Assert.AreEqual(expectedName, result);
        }
    }
}
