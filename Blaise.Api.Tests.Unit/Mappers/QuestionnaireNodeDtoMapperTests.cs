namespace Blaise.Api.Tests.Unit.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Mappers;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class QuestionnaireNodeDtoMapperTests
    {
        private QuestionnaireNodeDtoMapper _sut;

        [SetUp]
        public void SetupTests()
        {
            _sut = new QuestionnaireNodeDtoMapper();
        }

        [Test]
        public void Given_A_Multi_Node_Setup_When_I_Call_MapToQuestionnaireNodeDtos_Then_A_Populated_Node_List_Is_Returned()
        {
            // arrange

            // node 1
            const string Node1Name = "data-management";
            var node1Status = QuestionnaireStatusType.Active.ToString();
            var iConfiguration1Mock = new Mock<IConfiguration>();
            iConfiguration1Mock.Setup(c => c.Status).Returns(node1Status);

            // node 2
            const string Node2Name = "data-entry1";
            var node2Status = QuestionnaireStatusType.Installing.ToString();
            var iConfiguration2Mock = new Mock<IConfiguration>();
            iConfiguration2Mock.Setup(c => c.Status).Returns(node2Status);

            // multi node setup
            var machineConfigurationMock = new Mock<IMachineConfigurationCollection>();
            var machineConfigurations = new List<KeyValuePair<string, IConfiguration>>
            {
                new KeyValuePair<string, IConfiguration>(Node1Name, iConfiguration1Mock.Object),
                new KeyValuePair<string, IConfiguration>(Node2Name, iConfiguration2Mock.Object),
            };

            machineConfigurationMock.Setup(m => m.GetEnumerator())
                .Returns(machineConfigurations.GetEnumerator());

            // act
            var result = _sut.MapToQuestionnaireNodeDtos(machineConfigurationMock.Object)
                .ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<QuestionnaireNodeDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(n => n.NodeName == Node1Name && n.NodeStatus == node1Status), Is.True);
            Assert.That(result.Any(n => n.NodeName == Node2Name && n.NodeStatus == node2Status), Is.True);
        }
    }
}
