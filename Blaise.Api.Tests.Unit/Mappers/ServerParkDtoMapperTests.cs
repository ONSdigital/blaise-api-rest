using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Questionnaire;
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
        private Mock<IQuestionnaireDtoMapper> _questionnaireDtoMapperMock;

        [SetUp]
        public void SetupTests()
        {
            _questionnaireDtoMapperMock = new Mock<IQuestionnaireDtoMapper>();

            _sut = new ServerParkDtoMapper(_questionnaireDtoMapperMock.Object);
        }

        [Test]
        public void Given_A_List_Of_ServerParks_When_I_Call_MapToServerParkDtos_Then_A_Correct_List_Of_ServerParkDto_Is_Returned()
        {
            // arrange
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

            // act
            var result = _sut.MapToServerParkDtos(serverParks).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<ServerParkDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.True(result.Any(i => i.Name == serverPark1Name));
            Assert.True(result.Any(i => i.Name == serverPark2Name));
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_MapToServerParkDto_Then_A_Correct_ServerParkDto_Is_Returned()
        {
            // arrange
            var productVersionInfoMock = new Mock<IProductVersionInfo>();
            productVersionInfoMock.Setup(pi => pi.ToString()).Returns("5.9.9");

            const string serverParkName = "ServerParkA";
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Name).Returns(serverParkName);

            var server1Mock = new Mock<IServer>();
            const string machine1Name = "ServerA";
            const string machine1LogicalRoot = "Default1";
            var machine1Roles = new List<string> { "role1", "role2" };
            var machine1ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine1ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine1Roles.GetEnumerator());
            server1Mock.Setup(s => s.Name).Returns(machine1Name);
            server1Mock.As<IServer2>().Setup(s => s.BlaiseVersion).Returns(productVersionInfoMock.Object);
            server1Mock.Setup(s => s.LogicalRoot).Returns(machine1LogicalRoot);
            server1Mock.Setup(s => s.Roles).Returns(machine1ServerRoleCollection.Object);

            var server2Mock = new Mock<IServer>();
            const string machine2Name = "ServerB";
            const string machine2LogicalRoot = "Default2";
            var machine2Roles = new List<string> { "role3" };
            var machine2ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine2ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine2Roles.GetEnumerator());
            server2Mock.Setup(s => s.Name).Returns(machine2Name);
            server2Mock.As<IServer2>().Setup(s => s.BlaiseVersion).Returns(productVersionInfoMock.Object);
            server2Mock.Setup(s => s.LogicalRoot).Returns(machine2LogicalRoot);
            server2Mock.Setup(s => s.Roles).Returns(machine2ServerRoleCollection.Object);

            var serverList = new List<IServer> { server1Mock.Object, server2Mock.Object };
            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(serverList.GetEnumerator());

            serverPark.Setup(s => s.Servers).Returns(serverCollection.Object);

            // act
            var result = _sut.MapToServerParkDto(serverPark.Object);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ServerParkDto>(result);
            Assert.AreEqual(serverParkName, result.Name);

            Assert.IsNotNull(result.Servers);
            Assert.IsInstanceOf<IEnumerable<ServerDto>>(result.Servers);
            Assert.IsNotEmpty(result.Servers);
            Assert.AreEqual(2, result.Servers.Count());

            Assert.True(result.Servers.Any(s => s.Name == machine1Name && s.BlaiseVersion == "5.9.9" && s.LogicalServerName == machine1LogicalRoot &&
                                                s.Roles.OrderByDescending(l => l).SequenceEqual(machine1Roles.OrderByDescending(l => l))));

            Assert.True(result.Servers.Any(s => s.Name == machine2Name && s.BlaiseVersion == "5.9.9" && s.LogicalServerName == machine2LogicalRoot &&
                                                s.Roles.OrderByDescending(l => l).SequenceEqual(machine2Roles.OrderByDescending(l => l))));
        }

        [Test]
        public void Given_A_ServerPark_Has_A_Questionnaire_When_I_Call_MapToServerParkDto_Then_The_Questionnaire_Is_Mapped()
        {
            // arrange
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Surveys).Returns(It.IsAny<ISurveyCollection>());

            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(new List<IServer>().GetEnumerator());
            serverPark.Setup(s => s.Servers).Returns(serverCollection.Object);

            var questionnaire1Dto = new QuestionnaireDto
            {
                Name = "OPN2010A",
                ServerParkName = "ServerParkA",
                InstallDate = DateTime.Today.AddDays(-2),
                Status = QuestionnaireStatusType.Inactive.FullName()
            };

            var questionnaire2Dto = new QuestionnaireDto
            {
                Name = "OPN2010B",
                ServerParkName = "ServerParkB",
                InstallDate = DateTime.Today,
                Status = QuestionnaireStatusType.Inactive.FullName()
            };

            _questionnaireDtoMapperMock.Setup(m => m.MapToQuestionnaireDtos(
                    It.IsAny<ISurveyCollection>()))
                .Returns(new List<QuestionnaireDto> { questionnaire1Dto, questionnaire2Dto });

            // act
            var result = _sut.MapToServerParkDto(serverPark.Object).Questionnaires.ToList();

            // assert
            Assert.IsInstanceOf<List<QuestionnaireDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(i => i.Name == questionnaire1Dto.Name && i.ServerParkName == questionnaire1Dto.ServerParkName &&
                                        i.InstallDate == questionnaire1Dto.InstallDate && i.Status == questionnaire1Dto.Status));

            Assert.True(result.Any(i => i.Name == questionnaire2Dto.Name && i.ServerParkName == questionnaire2Dto.ServerParkName &&
                                        i.InstallDate == questionnaire2Dto.InstallDate && i.Status == questionnaire2Dto.Status));
        }
    }
}
