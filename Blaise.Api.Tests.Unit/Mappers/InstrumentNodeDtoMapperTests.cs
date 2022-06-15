using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
{
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
            //arrange
           
            //node 1
            const string node1Name = "data-management";
            var node1Status = QuestionnaireStatusType.Active.ToString();
            var iConfiguration1Mock = new Mock<IConfiguration>();
            iConfiguration1Mock.Setup(c => c.Status).Returns(node1Status);

            //node 2
            const string node2Name = "data-entry1";
            var node2Status = QuestionnaireStatusType.Installing.ToString();
            var iConfiguration2Mock = new Mock<IConfiguration>();
            iConfiguration2Mock.Setup(c => c.Status).Returns(node2Status);

            //multi node setup
            var machineConfigurationMock = new Mock<IMachineConfigurationCollection>();
            var machineConfigurations = new List<KeyValuePair<string, IConfiguration>>
            {
                new KeyValuePair<string, IConfiguration>(node1Name, iConfiguration1Mock.Object),
                new KeyValuePair<string, IConfiguration>(node2Name, iConfiguration2Mock.Object)
            };

            machineConfigurationMock.Setup(m => m.GetEnumerator())
                .Returns(machineConfigurations.GetEnumerator());
            
            //act
            var result = _sut.MapToQuestionnaireNodeDtos(machineConfigurationMock.Object)
                .ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<QuestionnaireNodeDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(n => n.NodeName == node1Name && n.NodeStatus == node1Status));
            Assert.True(result.Any(n => n.NodeName == node2Name && n.NodeStatus == node2Status));
        }
    }
}
