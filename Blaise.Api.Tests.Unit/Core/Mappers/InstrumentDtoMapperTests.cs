using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Mappers;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Core.Mappers
{
    public class InstrumentDtoMapperTests
    {
        private InstrumentDtoMapper _sut;

        [SetUp]
        public void SetupTests()
        {
            _sut = new InstrumentDtoMapper();
        }

        [Test]
        public void Given_A_List_Of_Surveys_When_I_Call_MapToDto_Then_A_Correct_List_Of_Instrument_Dtos_Is_Returned()
        {
            //arrange
            var instrument1Name = "OPN2010A";
            var instrument2Name = "OPN2010B";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(instrument1Name);

            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(instrument2Name);

            var surveys = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object
            };

            //act
            var result = _sut.MapToDto(surveys).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
            Assert.AreEqual(2, result.Count());
            Assert.True(result.Any(i => i.Name == instrument1Name));
            Assert.True(result.Any(i => i.Name == instrument2Name));
        }

        [Test]
        public void Given_A_Survey_When_I_Call_MapToDto_Then_A_Correct_Instrument_Dto_Is_Returned()
        {
            //arrange
            var instrumentName = "OPN2010A";
            var serverParkName = "ServerParkA";
            var surveyMock = new Mock<ISurvey>();
            surveyMock.Setup(s => s.Name).Returns(instrumentName);
            surveyMock.Setup(s => s.ServerPark).Returns(serverParkName);

            //act
            var result = _sut.MapToDto(surveyMock.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InstrumentDto>(result);
            Assert.AreEqual(instrumentName,result.Name);
            Assert.AreEqual(serverParkName,result.ServerParkName);
        }
    }
}
