using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class QuestionnaireDtoMapperTests
    {
        private QuestionnaireDtoMapper _sut;
        private Mock<IQuestionnaireStatusMapper> _statusMapperMock;
        private Mock<IQuestionnaireNodeDtoMapper> _nodeDtoMapperMock;

        private DateTime _installDate;
        private int _numberOfRecordForQuestionnaire;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;
        
        [SetUp]
        public void SetupTests()
        {
            _installDate = DateTime.Now;
            _numberOfRecordForQuestionnaire = 100;

            _surveyMock = new Mock<ISurvey>();
            _surveyMock.Setup(s => s.InstallDate).Returns(_installDate);

            _surveyReportingInfoMock = new Mock<ISurveyReportingInfo>();
            _surveyReportingInfoMock.Setup(r => r.DataRecordCount).Returns(_numberOfRecordForQuestionnaire);
            _surveyMock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(_surveyReportingInfoMock.Object);

            _statusMapperMock = new Mock<IQuestionnaireStatusMapper>();
            _nodeDtoMapperMock = new Mock<IQuestionnaireNodeDtoMapper>();

            _sut = new QuestionnaireDtoMapper(_statusMapperMock.Object, _nodeDtoMapperMock.Object);
        }

        [Test]
        public void Given_A_Survey_When_I_Call_MapToQuestionnaireDto_Then_Properties_Are_Mapped_Correctly()
        {
            //arrange
            const string questionnaire1Name = "OPN2010A";
            var questionnaire1Id = Guid.NewGuid();
            const string serverPark1Name = "ServerParkA";

            const int numberOfRecordForQuestionnaire1 = 20;

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForQuestionnaire1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);
            
            _statusMapperMock.Setup(s => s.GetQuestionnaireStatus(survey1Mock.Object))
                .Returns(QuestionnaireStatusType.Active);

            var nodeList = new List<QuestionnaireNodeDto>
            {
                new QuestionnaireNodeDto()
            };

            _nodeDtoMapperMock.Setup(n => n.MapToQuestionnaireNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            //act
            var result = _sut.MapToQuestionnaireDto(survey1Mock.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<QuestionnaireDto>(result);
            Assert.AreEqual(questionnaire1Name, result.Name);
            Assert.AreEqual(questionnaire1Id, result.Id);
            Assert.AreEqual(serverPark1Name, result.ServerParkName);
            Assert.AreEqual(numberOfRecordForQuestionnaire1, result.DataRecordCount);
            Assert.AreEqual(QuestionnaireStatusType.Active.ToString(), result.Status);
            Assert.True(result.HasData);
            Assert.AreSame(nodeList, result.Nodes);
        }

        [Test]
        public void Given_A_List_Of_Surveys_When_I_Call_MapToQuestionnaireDtos_Then_The_General_Properties_Are_Mapped()
        {
            //arrange
            const string questionnaire1Name = "OPN2010A";
            const string questionnaire2Name = "OPN2010B";

            var questionnaire1Id = Guid.NewGuid();
            var questionnaire2Id = Guid.NewGuid();

            const string serverPark1Name = "ServerParkA";
            const string serverPark2Name = "ServerParkB";

            const int numberOfRecordForQuestionnaire1 = 20;
            const int numberOfRecordForQuestionnaire2 = 0;

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForQuestionnaire1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);

            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.As<ISurvey2>();
            survey2Mock.Setup(s => s.Name).Returns(questionnaire2Name);
            survey2Mock.Setup(s => s.InstrumentID).Returns(questionnaire2Id);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark2Name);

            var surveyReportingInfoMock2 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock2.Setup(r => r.DataRecordCount).Returns(numberOfRecordForQuestionnaire2);
            survey2Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock2.Object);
            
            var surveys = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object
            };

            _statusMapperMock.Setup(s => s.GetQuestionnaireStatus(survey1Mock.Object))
                .Returns(QuestionnaireStatusType.Active);

            _statusMapperMock.Setup(s => s.GetQuestionnaireStatus(survey2Mock.Object))
                .Returns(QuestionnaireStatusType.Installing);

            var nodeList = new List<QuestionnaireNodeDto>
            {
                new QuestionnaireNodeDto(),
                new QuestionnaireNodeDto()
            };

            _nodeDtoMapperMock.Setup(n => n.MapToQuestionnaireNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            //act
            var result = _sut.MapToQuestionnaireDtos(surveys).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<QuestionnaireDto>>(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(i =>
                i.Name == questionnaire1Name &&
                i.Id == questionnaire1Id &&
                i.ServerParkName == serverPark1Name &&
                i.DataRecordCount == numberOfRecordForQuestionnaire1 &&
                i.Status == QuestionnaireStatusType.Active.ToString() &&
                i.HasData &&
                i.Nodes.Count() == 2));

            Assert.True(result.Any(i =>
                i.Name == questionnaire2Name &&
                i.Id == questionnaire2Id &&
                i.ServerParkName == serverPark2Name &&
                i.DataRecordCount == numberOfRecordForQuestionnaire2 &&
                i.Status == QuestionnaireStatusType.Installing.ToString() &&
                i.HasData == false &&
                i.Nodes.Count() == 2));
        }
    }
}
