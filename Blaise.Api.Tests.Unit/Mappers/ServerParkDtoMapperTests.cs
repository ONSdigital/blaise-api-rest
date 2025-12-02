namespace Blaise.Api.Tests.Unit.Mappers
{
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

            const string ServerPark1Name = "ServerParkA";
            var serverPark1 = new Mock<IServerPark>();
            serverPark1.Setup(s => s.Name).Returns(ServerPark1Name);
            serverPark1.Setup(s => s.Servers).Returns(serverCollection.Object);

            const string ServerPark2Name = "ServerParkA";
            var serverPark2 = new Mock<IServerPark>();
            serverPark2.Setup(s => s.Name).Returns(ServerPark2Name);
            serverPark2.Setup(s => s.Servers).Returns(It.IsAny<IServerCollection>());
            serverPark2.Setup(s => s.Servers).Returns(serverCollection.Object);

            var serverParks = new List<IServerPark>
            {
                serverPark1.Object,
                serverPark2.Object,
            };

            // act
            var result = _sut.MapToServerParkDtos(serverParks).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<ServerParkDto>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(i => i.Name == ServerPark1Name), Is.True);
            Assert.That(result.Any(i => i.Name == ServerPark2Name), Is.True);
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_MapToServerParkDto_Then_A_Correct_ServerParkDto_Is_Returned()
        {
            // arrange
            var productVersionInfoMock = new Mock<IProductVersionInfo>();
            productVersionInfoMock.Setup(pi => pi.ToString()).Returns("5.9.9");

            const string ServerParkName = "ServerParkA";
            var serverPark = new Mock<IServerPark>();
            serverPark.Setup(s => s.Name).Returns(ServerParkName);

            var server1Mock = new Mock<IServer>();
            const string Machine1Name = "ServerA";
            const string Machine1LogicalRoot = "Default1";
            var machine1Roles = new List<string> { "role1", "role2" };
            var machine1ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine1ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine1Roles.GetEnumerator());
            server1Mock.Setup(s => s.Name).Returns(Machine1Name);
            server1Mock.As<IServer2>().Setup(s => s.BlaiseVersion).Returns(productVersionInfoMock.Object);
            server1Mock.Setup(s => s.LogicalRoot).Returns(Machine1LogicalRoot);
            server1Mock.Setup(s => s.Roles).Returns(machine1ServerRoleCollection.Object);

            var server2Mock = new Mock<IServer>();
            const string Machine2Name = "ServerB";
            const string Machine2LogicalRoot = "Default2";
            var machine2Roles = new List<string> { "role3" };
            var machine2ServerRoleCollection = new Mock<IServerRoleCollection>();
            machine2ServerRoleCollection.Setup(s => s.GetEnumerator()).Returns(machine2Roles.GetEnumerator());
            server2Mock.Setup(s => s.Name).Returns(Machine2Name);
            server2Mock.As<IServer2>().Setup(s => s.BlaiseVersion).Returns(productVersionInfoMock.Object);
            server2Mock.Setup(s => s.LogicalRoot).Returns(Machine2LogicalRoot);
            server2Mock.Setup(s => s.Roles).Returns(machine2ServerRoleCollection.Object);

            var serverList = new List<IServer> { server1Mock.Object, server2Mock.Object };
            var serverCollection = new Mock<IServerCollection>();
            serverCollection.Setup(s => s.GetEnumerator()).Returns(serverList.GetEnumerator());

            serverPark.Setup(s => s.Servers).Returns(serverCollection.Object);

            // act
            var result = _sut.MapToServerParkDto(serverPark.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ServerParkDto>());
            Assert.That(result.Name, Is.EqualTo(ServerParkName));
            Assert.That(result.Servers, Is.Not.Null);
            Assert.That(result.Servers, Is.InstanceOf<IEnumerable<ServerDto>>());
            Assert.That(result.Servers, Is.Not.Empty);
            Assert.That(result.Servers.Count(), Is.EqualTo(2));

            Assert.That(
                result.Servers.Any(
                    s => s.Name == Machine1Name &&
                    s.BlaiseVersion == "5.9.9" &&
                    s.LogicalServerName == Machine1LogicalRoot &&
                    s.Roles.OrderByDescending(l => l).SequenceEqual(machine1Roles.OrderByDescending(l => l))), Is.True);

            Assert.That(
                result.Servers.Any(
                    s => s.Name == Machine2Name &&
                    s.BlaiseVersion == "5.9.9" &&
                    s.LogicalServerName == Machine2LogicalRoot &&
                    s.Roles.OrderByDescending(l => l).SequenceEqual(machine2Roles.OrderByDescending(l => l))), Is.True);
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
                Status = QuestionnaireStatusType.Inactive.FullName(),
            };

            var questionnaire2Dto = new QuestionnaireDto
            {
                Name = "OPN2010B",
                ServerParkName = "ServerParkB",
                InstallDate = DateTime.Today,
                Status = QuestionnaireStatusType.Inactive.FullName(),
            };

            _questionnaireDtoMapperMock.Setup(m => m.MapToQuestionnaireDtos(
                    It.IsAny<ISurveyCollection>()))
                .Returns(new List<QuestionnaireDto> { questionnaire1Dto, questionnaire2Dto });

            // act
            var result = _sut.MapToServerParkDto(serverPark.Object).Questionnaires.ToList();

            // assert
            Assert.That(result, Is.InstanceOf<List<QuestionnaireDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result.Any(
                    i => i.Name == questionnaire1Dto.Name &&
                    i.ServerParkName == questionnaire1Dto.ServerParkName &&
                    i.InstallDate == questionnaire1Dto.InstallDate &&
                    i.Status == questionnaire1Dto.Status), Is.True);

            Assert.That(
                result.Any(
                    i => i.Name == questionnaire2Dto.Name &&
                    i.ServerParkName == questionnaire2Dto.ServerParkName &&
                    i.InstallDate == questionnaire2Dto.InstallDate &&
                    i.Status == questionnaire2Dto.Status), Is.True);
        }
    }
}
