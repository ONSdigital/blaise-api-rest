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
    public class ServerParkServiceTests
    {
        private Mock<IBlaiseServerParkApi> _blaiseApiMock;
        private Mock<IServerParkDtoMapper> _mapperMock;
        private IServerParkService _sut;

        [SetUp]
        public void SetupTests()
        {
            _blaiseApiMock = new Mock<IBlaiseServerParkApi>();

            _mapperMock = new Mock<IServerParkDtoMapper>();

            _sut = new ServerParkService(_blaiseApiMock.Object, _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetServerParkNames_Then_The_Correct_Method_Is_called_On_The_Api()
        {
            //act
            _sut.GetServerParkNames();

            //assert
            _blaiseApiMock.Verify(v => v.GetNamesOfServerParks(), Times.Once);
        }

        [Test]
        public void Given_I_Call_GetServerParkNames_Then_The_Correct_ServerParkNames_Are_Returned()
        {
            //arrange
            var serverParkNames = new List<string>
            {
                "ServerParkA",
                "ServerParkB"
            };

            _blaiseApiMock.Setup(b => b.GetNamesOfServerParks()).Returns(serverParkNames);

            //act
            var result = _sut.GetServerParkNames().ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<string>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(serverParkNames, result);
        }

        [Test]
        public void Given_I_Call_GetServerParks_Then_The_Correct_Method_Is_called_On_The_Api()
        {
            //arrange
            var serverPark1Mock = new Mock<IServerPark>();
            serverPark1Mock.Setup(s => s.Name).Returns("ServerParkA");
            var serverPark2Mock = new Mock<IServerPark>();
            serverPark2Mock.Setup(s => s.Name).Returns("ServerParkB");

            var serverParkList = new List<IServerPark>
            {
                serverPark1Mock.Object,
                serverPark2Mock.Object
            };

            _blaiseApiMock.Setup(b => b.GetServerParks()).Returns(serverParkList);

            //act
            _sut.GetServerParks();

            //assert
            _blaiseApiMock.Verify(v => v.GetServerParks(), Times.Once);
            _mapperMock.Verify(v => v.MapToDto(serverParkList));
        }

        [Test]
        public void Given_I_Call_GetServerParks_Then_The_Correct_ServerParkDto_List_Is_returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetServerParks()).Returns(new List<IServerPark>());
            var serverParkDtoList = new List<ServerParkDto>
            {
                new ServerParkDto(),
                new ServerParkDto()
            };

            _mapperMock.Setup(m => m.MapToDto(It.IsAny<List<IServerPark>>()))
                .Returns(serverParkDtoList);

            //act
            var result = _sut.GetServerParks().ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ServerParkDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(serverParkDtoList, result);
        }

        [Test]
        public void Given_I_Call_GetServerPark_Then_The_Correct_Method_Is_called_On_The_Api()
        {
            //arrange
            var serverParkName = "ServerParkA";
            var serverPark1Mock = new Mock<IServerPark>();
            serverPark1Mock.Setup(s => s.Name).Returns(serverParkName);

            _blaiseApiMock.Setup(b => b.GetServerPark(serverParkName))
                .Returns(serverPark1Mock.Object);

            //act
            _sut.GetServerPark(serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetServerPark(serverParkName), Times.Once);
            _mapperMock.Verify(v => v.MapToDto(serverPark1Mock.Object));
        }

        [Test]
        public void Given_I_Call_GetServerPark_Then_The_Correct_ServerParkDto_Is_returned()
        {
            //arrange
            var serverParkName = "ServerParkA";
            var serverParkDto = new ServerParkDto();

            _blaiseApiMock.Setup(b => b.GetServerPark(serverParkName))
                .Returns(It.IsAny<IServerPark>());

            _mapperMock.Setup(m => m.MapToDto(It.IsAny<IServerPark>()))
                .Returns(serverParkDto);

            //act
            var result = _sut.GetServerPark(serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServerParkDto>(result);
            Assert.AreEqual(serverParkDto, result);
        }

        [Test]
        public void Given_I_Call_ServerParkExists_Then_The_Correct_Method_Is_called_On_The_Api()
        {
            //arrange
            var serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b => b.ServerParkExists(serverParkName))
                .Returns(It.IsAny<bool>());

            //act
            _sut.ServerParkExists(serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.ServerParkExists(serverParkName), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_I_Call_ServerParkExists_Then_The_Correct_Response_Is_returned(bool exists)
        {
            //arrange
            var serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b => b.ServerParkExists(serverParkName))
                .Returns(exists);

            //act
            var result = _sut.ServerParkExists(serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(exists, result);
        }
    }
}
