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
        
        private string _instrumentName;
        private string _instrumentFile;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseFileApiMock = new Mock<IBlaiseFileApi>();

            _fileSystemMock = new MockFileSystem();

            _instrumentFile = "OPN2010A.zip";
            _instrumentName = "OPN2010A";

            _sut = new FileService(_blaiseFileApiMock.Object, _fileSystemMock);
        }

        [Test]
        public void Given_I_Call_UpdateInstrumentFileWithSqlConnection_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            _blaiseFileApiMock.Setup(b => b.UpdateInstrumentFileWithSqlConnection(
                _instrumentName, _instrumentFile));

            //act
            _sut.UpdateInstrumentFileWithSqlConnection(_instrumentFile);

            //assert
            _blaiseFileApiMock.Verify(v => v.UpdateInstrumentFileWithSqlConnection(_instrumentName,
                _instrumentFile), Times.Once);
        }

        [Test]
        public void Given_An_Empty_InstrumentFile_When_I_Call_UpdateInstrumentFileWithSqlConnection_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateInstrumentFileWithSqlConnection(string.Empty));
            Assert.AreEqual("A value for the argument 'instrumentFile' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentFile_When_I_Call_UpdateInstrumentFileWithSqlConnection_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateInstrumentFileWithSqlConnection(null));
            Assert.AreEqual("instrumentFile", exception.ParamName);
        }

        [Test]
        public void Given_I_Call_GetInstrumentNameFromFile_Then_The_Correct_Name_Is_Returned()
        {
            //act
            var result = _sut.GetInstrumentNameFromFile(_instrumentFile);

            //assert
            Assert.AreEqual(_instrumentName, result);
        }

        [Test]
        public void Given_I_Call_GetDatabaseFile_Then_The_Correct_Name_Is_Returned()
        {
            //arrange
            var filePath = @"d:\test";
            var expectedName = $@"{filePath}\{_instrumentName}.bdix";

            //act
            var result = _sut.GetDatabaseFile(filePath, _instrumentName);

            //assert
            Assert.AreEqual(expectedName, result);
        }
    }
}
