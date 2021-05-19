using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Extensions;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
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
        public void Given_A_List_Of_ServerParks_When_I_Call_MapToServerParkDtos_Then_A_Correct_List_Of_ServerParkDto_Is_Returned()
        {
            //arrange
            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(new List<IServer>().GetEnumerator());

            const string serverPark1Name = "ServerParkA";
            var serverPark1 = new Mock<IServerPark>();
            serverPark1.Setup(s => s.Name).Returns(serverPark1Name);
            serverPark1.Setup(s => s.Servers).Returns(serverCollection.Object);

            const string serverPark2Name = "ServerParkA";
            var serverPark2 = new Mock<IServerPark>();
            serverPark2.Setup(s => s.Name).Returns(serverPark2Name);
            serverPark2.Setup(s => s.Servers).Returns(It.IsAny<IServerCollection>());
            serverPark2.Setup(s => s.Servers).Returns(serverCollection.Object);

            var serverParks = new List<IServerPark>
            {
                serverPark1.Object,
                serverPark2.Object
            };

            //act
            var result = _sut.MapToServerParkDtos(serverParks).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ServerParkDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.True(result.Any(i => i.Name == serverPark1Name));
            Assert.True(result.Any(i => i.Name == serverPark2Name));
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_MapToServerParkDto_Then_A_Correct_ServerParkDto_Is_Returned()
        {
            //arrange
            const string serverParkName = "ServerParkA";
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Name).Returns(serverParkName);

            var server1Mock = new Mock<IServer>();
            const string machine1Name = "ServerA";
            const string machine1LogicalRoot = "Default1";
            var machine1Roles = new List<string> {"role1", "role2"};
            var machine1ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine1ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine1Roles.GetEnumerator());
            server1Mock.Setup(s => s.Name).Returns(machine1Name);
            server1Mock.Setup(s => s.LogicalRoot).Returns(machine1LogicalRoot);
            server1Mock.Setup(s => s.Roles).Returns(machine1ServerRoleCollection.Object);

            var server2Mock = new Mock<IServer>();
            const string machine2Name = "ServerB";  
            const string machine2LogicalRoot = "Default2";
            var machine2Roles = new List<string> {"role3"};
            var machine2ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine2ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine2Roles.GetEnumerator());
            server2Mock.Setup(s => s.Name).Returns(machine2Name);
            server2Mock.Setup(s => s.LogicalRoot).Returns(machine2LogicalRoot);
            server2Mock.Setup(s => s.Roles).Returns(machine2ServerRoleCollection.Object);

            var serverList = new List<IServer> { server1Mock.Object, server2Mock.Object};
            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(serverList.GetEnumerator());

            serverPark.Setup(s => s.Servers).Returns(serverCollection.Object);
            
            //act
            var result = _sut.MapToServerParkDto(serverPark.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServerParkDto>(result);
            Assert.AreEqual(serverParkName, result.Name);

            Assert.IsNotNull(result.Servers);
            Assert.IsInstanceOf<IEnumerable<ServerDto>>(result.Servers);
            Assert.IsNotEmpty(result.Servers);
            Assert.AreEqual(2, result.Servers.Count());

            Assert.True(result.Servers.Any(s => s.Name == machine1Name && s.LogicalServerName == machine1LogicalRoot &&
                                                s.Roles.OrderByDescending(l => l).SequenceEqual(machine1Roles.OrderByDescending(l => l))));

            Assert.True(result.Servers.Any(s => s.Name == machine2Name && s.LogicalServerName == machine2LogicalRoot && 
                                                s.Roles.OrderByDescending(l => l).SequenceEqual(machine2Roles.OrderByDescending(l => l))));

        }

        [Test]
        public void Given_A_ServerPark_Has_An_Instrument_When_I_Call_MapToServerParkDto_Then_The_Instrument_Is_Mapped()
        {
            //arrange
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Surveys).Returns(It.IsAny<ISurveyCollection>());

            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(new List<IServer>().GetEnumerator());
            serverPark.Setup(s => s.Servers).Returns(serverCollection.Object);

            var instrument1Dto = new InstrumentDto
            {
                Name = "OPN2010A",
                ServerParkName = "ServerParkA",
                InstallDate = DateTime.Today.AddDays(-2),
                Status = SurveyStatusType.Inactive.FullName()
            };

            var instrument2Dto = new InstrumentDto
            {
                Name = "OPN2010B",
                ServerParkName = "ServerParkB",
                InstallDate = DateTime.Today,
                Status = SurveyStatusType.Inactive.FullName()
            };

            _instrumentDtoMapperMock.Setup(m => m.MapToInstrumentDtos(
                    It.IsAny<ISurveyCollection>()))
                .Returns(new List<InstrumentDto> { instrument1Dto, instrument2Dto });

            //act
            var result = _sut.MapToServerParkDto(serverPark.Object).Instruments.ToList();

            //assert
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(i => i.Name == instrument1Dto.Name && i.ServerParkName == instrument1Dto.ServerParkName && 
                                        i.InstallDate == instrument1Dto.InstallDate && i.Status == instrument1Dto.Status));

            Assert.True(result.Any(i => i.Name == instrument2Dto.Name && i.ServerParkName == instrument2Dto.ServerParkName && 
                                        i.InstallDate == instrument2Dto.InstallDate && i.Status == instrument2Dto.Status));
        }
    }
}
