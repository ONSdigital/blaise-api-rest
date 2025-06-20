using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Services
{
    public class ServerParkServiceTests
    {
        private Mock<IBlaiseServerParkApi> _blaiseApiMock;
        private Mock<IServerParkDtoMapper> _mapperMock;
        private IServerParkService _sut;

        private string _serverParkName;

        [SetUp]
        public void SetupTests()
        {
            _serverParkName = "Park1";

            _blaiseApiMock = new Mock<IBlaiseServerParkApi>();

            _mapperMock = new Mock<IServerParkDtoMapper>();

            _sut = new ServerParkService(_blaiseApiMock.Object, _mapperMock.Object);
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
            _mapperMock.Verify(v => v.MapToServerParkDtos(serverParkList));
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

            _mapperMock.Setup(m => m.MapToServerParkDtos(It.IsAny<List<IServerPark>>()))
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
            var serverPark1Mock = new Mock<IServerPark>();
            serverPark1Mock.Setup(s => s.Name).Returns(_serverParkName);

            _blaiseApiMock.Setup(b => b.GetServerPark(_serverParkName))
                .Returns(serverPark1Mock.Object);

            //act
            _sut.GetServerPark(_serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetServerPark(_serverParkName), Times.Once);
            _mapperMock.Verify(v => v.MapToServerParkDto(serverPark1Mock.Object));
        }

        [Test]
        public void Given_I_Call_GetServerPark_Then_The_Correct_ServerParkDto_Is_returned()
        {
            //arrange
            var serverParkDto = new ServerParkDto();

            _blaiseApiMock.Setup(b => b.GetServerPark(_serverParkName))
                .Returns(It.IsAny<IServerPark>());

            _mapperMock.Setup(m => m.MapToServerParkDto(It.IsAny<IServerPark>()))
                .Returns(serverParkDto);

            //act
            var result = _sut.GetServerPark(_serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServerParkDto>(result);
            Assert.AreEqual(serverParkDto, result);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetServerPark_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetServerPark(string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetServerPark_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetServerPark(null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_I_Call_ServerParkExists_Then_The_Correct_Method_Is_called_On_The_Api()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.ServerParkExists(_serverParkName))
                .Returns(It.IsAny<bool>());

            //act
            _sut.ServerParkExists(_serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.ServerParkExists(_serverParkName), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_I_Call_ServerParkExists_Then_The_Correct_Response_Is_returned(bool exists)
        {
            //arrange
            _blaiseApiMock.Setup(b => b.ServerParkExists(_serverParkName))
                .Returns(exists);

            //act
            var result = _sut.ServerParkExists(_serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<bool>(result);
            Assert.AreEqual(exists, result);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_ServerParkExists_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ServerParkExists(string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_ServerParkExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ServerParkExists(null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
