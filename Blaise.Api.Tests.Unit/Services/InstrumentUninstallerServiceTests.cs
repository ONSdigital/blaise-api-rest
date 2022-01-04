using System;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class InstrumentUninstallerServiceTests
    {
        private InstrumentUninstallerService _sut;

        private Mock<IBlaiseSurveyApi> _blaiseSurveyApiMock;
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;

        private string _serverParkName;
        private string _instrumentName;
        
        [SetUp]
        public void SetUpTests()
        {
            _blaiseSurveyApiMock = new Mock<IBlaiseSurveyApi>();
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();

            _serverParkName = "ServerParkA";
            _instrumentName = "OPN2010A";

            _sut = new InstrumentUninstallerService(
                _blaiseSurveyApiMock.Object,
                _blaiseCaseApiMock.Object);
        }

        [Test]
        public void Given_I_Call_UninstallInstrument_Then_The_Correct_Services_Are_Called()
        {
            //act
            _sut.UninstallInstrument(_instrumentName, _serverParkName);

            //assert
            _blaiseCaseApiMock.Verify(v => v.RemoveCases(_instrumentName, _serverParkName));
            _blaiseSurveyApiMock.Verify(v => v.UninstallSurvey(_instrumentName, _serverParkName, false)
                , Times.Once);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_UninstallInstrument_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UninstallInstrument(string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_UninstallInstrument_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UninstallInstrument(null,
                _serverParkName));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UninstallInstrument_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UninstallInstrument(_instrumentName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UninstallInstrument_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UninstallInstrument(_instrumentName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
