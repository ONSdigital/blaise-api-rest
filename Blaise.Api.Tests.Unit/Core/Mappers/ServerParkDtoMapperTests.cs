using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using Blaise.Api.Core.Mappers;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Core.Mappers
{
    public class ServerParkDtoMapperTests
    {
        private ServerParkDtoMapper _sut;
        private Mock<IInstrumentDtoMapper> _instrumentDtoMapperMock;

        [SetUp]
        public void SetupTests()
        {
            _instrumentDtoMapperMock = new Mock<IInstrumentDtoMapper>();

            _sut = new ServerParkDtoMapper(_instrumentDtoMapperMock.Object);
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_MapToDto_Then_A_Correct_ServerParkDto_Is_Returned()
        {
            //arrange
            var serverParkName = "ServerParkA";
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Name).Returns(serverParkName);

            //act
            var result = _sut.MapToDto(serverPark.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServerParkDto>(result);
            Assert.AreEqual(serverParkName, result.Name);
        }

        [Test]
        public void Given_A_ServerPark_Has_An_Instrument_When_I_Call_MapToDto_Then_The_Instrument_Is_Mapped()
        {
            //arrange
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Surveys).Returns(It.IsAny<ISurveyCollection>());
            
            var instrumentName = "OPN2010A";
            _instrumentDtoMapperMock.Setup(m => m.MapToDto(
                    It.IsAny<ISurveyCollection>()))
                .Returns(new List<InstrumentDto> { new InstrumentDto { Name = instrumentName }});

            //act
            var result = _sut.MapToDto(serverPark.Object).Instruments.ToList();

            //assert
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.Count);
            Assert.True(result.Any(i => i.Name == instrumentName));
        }

        [Test]
        public void Given_A_List_Of_ServerParks_When_I_Call_MapToDto_Then_A_Correct_List_Of_ServerParkDto_Is_Returned()
        {
            //arrange
            var serverPark1Name = "ServerParkA";
            var serverPark1 = new Mock<IServerPark>();
            serverPark1.Setup(s => s.Name).Returns(serverPark1Name);

            var serverPark2Name = "ServerParkA";
            var serverPark2 = new Mock<IServerPark>();
            serverPark2.Setup(s => s.Name).Returns(serverPark2Name);

            var serverParks = new List<IServerPark>
            {
                serverPark1.Object,
                serverPark2.Object
            };

            //act
            var result = (_sut.MapToDto(serverParks)).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ServerParkDto>>(result);
            Assert.AreEqual(2, result.Count());
            Assert.True(result.Any(i => i.Name == serverPark1Name));
            Assert.True(result.Any(i => i.Name == serverPark2Name));
        }
    }
}
