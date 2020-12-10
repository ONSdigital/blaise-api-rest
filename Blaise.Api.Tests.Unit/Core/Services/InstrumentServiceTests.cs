using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Core.Services
{
    public class InstrumentServiceTests
    {
        private IInstrumentService _sut;
        private Mock<IBlaiseSurveyApi> _blaiseApiMock;
        private Mock<IInstrumentDtoMapper> _mapperMock;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseSurveyApi>();
            _mapperMock = new Mock<IInstrumentDtoMapper>();

            _sut = new InstrumentService(
                _blaiseApiMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetAllInstruments_Then_I_Get_A_List_Of_InstrumentDtos_Back()
        {
            //arrange
            var surveys = new List<ISurvey>();
            
            _blaiseApiMock.Setup(b => b.GetSurveysAcrossServerParks())
                .Returns(surveys);

            _mapperMock.Setup(m => m.MapToDto(surveys))
                .Returns(new List<InstrumentDto>());
            //act
            var result = _sut.GetAllInstruments();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
        }
        
        [Test]
        public void Given_I_Call_GetAllInstruments_Then_I_Get_A_List_Of_All_Instruments_Across_All_ServerParks()
        {
            //arrange
            var surveys = new List<ISurvey>();

            _blaiseApiMock.Setup(b => b.GetSurveysAcrossServerParks())
                .Returns(surveys);

            //act
            _sut.GetAllInstruments();

            //assert
            _blaiseApiMock.Verify(b => b.GetSurveysAcrossServerParks());
        }

        [Test]
        public void Given_I_Call_GetAllInstruments_Then_I_Get_A_Correct_List_Of_InstrumentDtos()
        {
            //arrange
            var surveys = new List<ISurvey>();
            
            _blaiseApiMock.Setup(b => b.GetSurveysAcrossServerParks())
                .Returns(surveys);

            var instrumentDtos = new List<InstrumentDto>
            {
                new InstrumentDto {Name = "OPN2010A"},
                new InstrumentDto {Name = "OPN2010B"}
            };

            _mapperMock.Setup(m => m.MapToDto(surveys))
                .Returns(instrumentDtos);

            //act
            var result = _sut.GetAllInstruments().ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(instrumentDtos, result);
        }

        [Test]
        public void Given_I_Call_GetInstruments_Then_I_Get_A_List_Of_InstrumentDtos_Back()
        {
            //arrange
            var serverParkName = "ServerParkA";
            var instrumentDtos = new List<InstrumentDto>();
            var surveys = new List<ISurvey>();

            _blaiseApiMock.Setup(b => b.GetSurveys(serverParkName))
                .Returns(surveys);

            _mapperMock.Setup(m => m.MapToDto(surveys))
                .Returns(instrumentDtos);

            //act
            var result = _sut.GetInstruments(serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
        }

        [Test]
        public void Given_I_Call_GetInstrument_Then_I_Get_A_Correct_List_Of_InstrumentDtos_Back()
        {
            //arrange
            var serverParkName = "ServerParkA";
            var instrumentDtos = new List<InstrumentDto>();
            var surveys = new List<ISurvey>();

            _blaiseApiMock.Setup(b => b.GetSurveys(serverParkName))
                .Returns(surveys);

            _mapperMock.Setup(m => m.MapToDto(surveys))
                .Returns(instrumentDtos);

            //act
            var result = _sut.GetInstruments(serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(instrumentDtos, result);
        }

        [Test]
        public void Given_An_Instrument_Exists_When_I_Call_GetInstrument_Then_I_Get_An_InstrumentDto_Returned()
        {
            //arrange
            var instrumentName = "OPN2101A";
            var serverParkName = "ServerParkA";
            var instrumentDto = new InstrumentDto();
            var surveyMock = new Mock<ISurvey>();

            _blaiseApiMock.Setup(b => b
                    .GetSurvey(instrumentName, serverParkName))
                .Returns(surveyMock.Object);

            _mapperMock.Setup(m => m.MapToDto(surveyMock.Object))
                .Returns(instrumentDto);

            //act
            var result = _sut.GetInstrument(instrumentName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InstrumentDto>(result);
        }

        [Test]
        public void Given_An_Instrument_Exists_When_I_Call_GetInstrument_Then_I_Get_A_Correct_InstrumentDto_Returned()
        {
            //arrange
            var instrumentName = "OPN2101A";
            var serverParkName = "ServerParkA";
            var instrumentDto = new InstrumentDto();
            var surveyMock = new Mock<ISurvey>();

            _blaiseApiMock.Setup(b => b
                .GetSurvey(instrumentName, serverParkName))
                .Returns(surveyMock.Object);

            _mapperMock.Setup(m => m.MapToDto(surveyMock.Object))
                .Returns(instrumentDto);

            //act
            var result = _sut.GetInstrument(instrumentName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(instrumentDto, result);
        }

        [Test]
        public void Given_An_Instrument_Exists_When_I_Call_InstrumentExists_Then_True_Is_Retuned()
        {
            //arrange
            var instrumentName = "OPN2101A";
            var serverParkName = "ServerParkA";
            _blaiseApiMock.Setup(b =>
                b.SurveyExists(instrumentName, serverParkName)).Returns(true);

            //act
            var result = _sut.InstrumentExists(instrumentName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_An_Instrument_Does_Not_Exist_When_I_Call_InstrumentExists_Then_False_Is_Returned()
        {
            //arrange
            var instrumentName = "OPN2101A";
            var serverParkName = "ServerParkA";
            _blaiseApiMock.Setup(b =>
                b.SurveyExists(instrumentName, serverParkName)).Returns(false);

            //act
            var result = _sut.InstrumentExists(instrumentName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }
    }
}
