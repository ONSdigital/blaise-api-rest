using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class BlaiseFileServiceTests
    {
        private BlaiseFileService _sut;

        private Mock<IBlaiseFileApi> _blaiseFileApiMock;

        private IFileSystem _fileSystemMock;

        private Mock<IConfigurationProvider> _configurationProviderMock;

        private string _serverParkName;
        private string _instrumentName;
        private string _instrumentFile;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseFileApiMock = new Mock<IBlaiseFileApi>();

            _fileSystemMock = new MockFileSystem();

            _configurationProviderMock = new Mock<IConfigurationProvider>();

            _serverParkName = "ServerParkA";
            _instrumentFile = "OPN2010A.zip";
            _instrumentName = "OPN2010A";

            _sut = new BlaiseFileService(_blaiseFileApiMock.Object, _fileSystemMock, _configurationProviderMock.Object);
        }
                
        [Test]
        public void Given_I_Call_UpdateInstrumentFileWithData_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            _blaiseFileApiMock.Setup(b => b.UpdateInstrumentFileWithData(_serverParkName, _instrumentName,
                _instrumentFile));

            //act
            _sut.UpdateInstrumentFileWithData(_serverParkName, _instrumentFile);

            //assert
            _blaiseFileApiMock.Verify(v => v.UpdateInstrumentFileWithData(_serverParkName, _instrumentName, 
                    _instrumentFile),
                Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UpdateInstrumentFileWithData_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateInstrumentFileWithData(string.Empty,
                _instrumentFile));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UpdateInstrumentFileWithData_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateInstrumentFileWithData(null,
                _instrumentFile));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentFile_When_I_Call_UpdateInstrumentFileWithData_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateInstrumentFileWithData(_serverParkName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'instrumentFile' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentFile_When_I_Call_UpdateInstrumentFileWithData_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateInstrumentFileWithData(_serverParkName,
                null));
            Assert.AreEqual("instrumentFile", exception.ParamName);
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
        public void Given_I_Call_DeleteFile_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            const string instrumentFile = @"d:\temp\OPN2001A.zip";
            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(s => s.File.Delete(It.IsAny<string>()));

            var sut = new BlaiseFileService(_blaiseFileApiMock.Object, fileSystemMock.Object, _configurationProviderMock.Object);

            //act
            sut.DeleteFile(instrumentFile);

            //assert
            fileSystemMock.Verify(f =>f.File.Delete(instrumentFile));
        }

        [Test]
        public void Given_I_Call_GetInstrumentNameFromFile_Then_The_Correct_Services_Are_Called()
        {
            //act
            var result = _sut.GetInstrumentNameFromFile(_instrumentFile);

            //assert
            Assert.AreEqual(_instrumentName, result);
        }

        [Test]
        public void Given_I_Call_GetInstrumentPackageName_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            var packageExtension = "bpkg";
            var expectedPackageName = $"{_instrumentName}.{packageExtension}";

            _configurationProviderMock.Setup(c => c.PackageExtension).Returns(packageExtension);

            //act
            var result = _sut.GetInstrumentPackageName(_instrumentName);

            //assert
            Assert.AreEqual(expectedPackageName, result);
        }
    }
}
