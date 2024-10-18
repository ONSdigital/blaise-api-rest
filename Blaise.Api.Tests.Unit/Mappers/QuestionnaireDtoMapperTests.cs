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

        private int _numberOfRecordForQuestionnaire;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;

        [SetUp]
        public void SetupTests()
        {
            _numberOfRecordForQuestionnaire = 100;

            _surveyMock = new Mock<ISurvey>();
            
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
            const string questionnaire1Name = "FRS2404a";
            var questionnaire1Id = Guid.NewGuid();
            const string serverPark1Name = "ServerParkA";
            const string blaiseVersion = "5.9.9";
            var installDate = new DateTime(2024, 1, 1);
            var fieldPeriod = new DateTime(2024, 4, 1);
            var surveyTla = "FRS";

            const int numberOfRecordForQuestionnaire1 = 20;

            var configurationMock = new Mock<IConfiguration2>();
            configurationMock.Setup(c => c.BlaiseVersion).Returns(blaiseVersion);

            var configurations = new List<IConfiguration> { configurationMock.Object };

            var configurationCollectionMock = new Mock<IMachineConfigurationCollection>();
            configurationCollectionMock.Setup(cc => cc.Configurations).Returns(configurations);

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);
            survey1Mock.Setup(s => s.InstallDate).Returns(installDate);
            survey1Mock.Setup(s => s.Configuration).Returns(configurationCollectionMock.Object);

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
            Assert.AreEqual(blaiseVersion, result.BlaiseVersion);
            Assert.AreEqual(installDate, result.InstallDate);
            Assert.AreEqual(fieldPeriod, result.FieldPeriod);
            Assert.AreEqual(surveyTla, result.SurveyTla);
            Assert.AreSame(nodeList, result.Nodes);            
        }


        [TestCase(24, 0)]
        [TestCase(24, 13)]
        public void Given_A_Survey_With_An_Invalid_Field_Period_In_The_Name_When_I_Call_MapToQuestionnaireDto_Then_The_Field_Period_Is_Set_To_Null(int year, int month)
        {
            //arrange
            var questionnaire1Name = $"FRS{year}{month}T";
            var questionnaire1Id = Guid.NewGuid();
            const string serverPark1Name = "ServerParkA";
            const string blaiseVersion = "5.9.9";
            var installDate = new DateTime(2024, 2, 2);
            var surveyTla = "FRS";

            const int numberOfRecordForQuestionnaire1 = 20;

            var configurationMock = new Mock<IConfiguration2>();
            configurationMock.Setup(c => c.BlaiseVersion).Returns(blaiseVersion);

            var configurations = new List<IConfiguration> { configurationMock.Object };

            var configurationCollectionMock = new Mock<IMachineConfigurationCollection>();
            configurationCollectionMock.Setup(cc => cc.Configurations).Returns(configurations);

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);
            survey1Mock.Setup(s => s.InstallDate).Returns(installDate);
            survey1Mock.Setup(s => s.Configuration).Returns(configurationCollectionMock.Object);

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
            Assert.IsNull(result.FieldPeriod);
        }

        [Test]
        public void Given_A_Survey_Has_No_Configuration_Available_When_I_Call_MapToQuestionnaireDto_Then_BlaiseVersion_Is_Set_To_Default()
        {
            //arrange
            const string questionnaire1Name = "OPN2010A";
            var questionnaire1Id = Guid.NewGuid();
            const string serverPark1Name = "ServerParkA";

            const int numberOfRecordForQuestionnaire1 = 20;

            var configurations = new List<IConfiguration>();

            var configurationCollectionMock = new Mock<IMachineConfigurationCollection>();
            configurationCollectionMock.Setup(cc => cc.Configurations).Returns(configurations);

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);
            survey1Mock.Setup(s => s.Configuration).Returns(configurationCollectionMock.Object);

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
            Assert.AreEqual("Not Available", result.BlaiseVersion);
        }

        [Test]
        public void Given_A_List_Of_Surveys_When_I_Call_MapToQuestionnaireDtos_Then_The_General_Properties_Are_Mapped()
        {
            //arrange
            const string questionnaire1Name = "OPN2010A";
            const string questionnaire2Name = "OPN2010B";

            var fieldPeriod = new DateTime(2020, 10, 1);
            const string surveyTla = "OPN";

            var questionnaire1Id = Guid.NewGuid();
            var questionnaire2Id = Guid.NewGuid();

            const string serverPark1Name = "ServerParkA";
            const string serverPark2Name = "ServerParkB";

            const int numberOfRecordForQuestionnaire1 = 20;
            const int numberOfRecordForQuestionnaire2 = 0;

            const string questionnaire1BlaiseVersion = "5.9.9";
            const string questionnaire2BlaiseVersion = "5.9.3";

            var questionnaire1InstallDate = new DateTime(2024, 1, 1);
            var questionnaire2InstallDate = new DateTime(2024, 2, 1);

            var survey1Mock = new Mock<ISurvey>();

            var configuration1Mock = new Mock<IConfiguration2>();
            configuration1Mock.Setup(c => c.BlaiseVersion).Returns(questionnaire1BlaiseVersion);

            var configurations1 = new List<IConfiguration> { configuration1Mock.Object };

            var configurationCollection1Mock = new Mock<IMachineConfigurationCollection>();
            configurationCollection1Mock.Setup(cc => cc.Configurations).Returns(configurations1);

            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(questionnaire1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);
            survey1Mock.Setup(s => s.InstallDate).Returns(questionnaire1InstallDate);
            survey1Mock.Setup(s => s.Configuration).Returns(configurationCollection1Mock.Object);            

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForQuestionnaire1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);

            var survey2Mock = new Mock<ISurvey>();

            var configuration2Mock = new Mock<IConfiguration2>();
            configuration2Mock.Setup(c => c.BlaiseVersion).Returns(questionnaire2BlaiseVersion);

            var configurations2 = new List<IConfiguration> { configuration2Mock.Object };

            var configurationCollection2Mock = new Mock<IMachineConfigurationCollection>();
            configurationCollection2Mock.Setup(cc => cc.Configurations).Returns(configurations2);

            configuration2Mock.Setup(c => c.BlaiseVersion).Returns(questionnaire2BlaiseVersion);

            survey2Mock.As<ISurvey2>();
            survey2Mock.Setup(s => s.Name).Returns(questionnaire2Name);
            survey2Mock.Setup(s => s.InstrumentID).Returns(questionnaire2Id);
            survey2Mock.Setup(s => s.InstallDate).Returns(questionnaire2InstallDate);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark2Name);
            survey2Mock.Setup(s => s.Configuration).Returns(configurationCollection2Mock.Object);

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
                i.BlaiseVersion == questionnaire1BlaiseVersion &&
                i.InstallDate == questionnaire1InstallDate &&
                i.FieldPeriod == fieldPeriod &&
                i.SurveyTla == surveyTla &&
                i.Nodes.Count() == 2));

            Assert.True(result.Any(i =>
                i.Name == questionnaire2Name &&
                i.Id == questionnaire2Id &&
                i.ServerParkName == serverPark2Name &&
                i.DataRecordCount == numberOfRecordForQuestionnaire2 &&
                i.Status == QuestionnaireStatusType.Installing.ToString() &&
                i.HasData == false &&
                i.BlaiseVersion == questionnaire2BlaiseVersion &&
                i.InstallDate == questionnaire2InstallDate &&
                i.FieldPeriod == fieldPeriod &&
                i.SurveyTla == surveyTla &&
                i.Nodes.Count() == 2));
        }

        [Test]
        public void Given_I_Call_GetFieldPeriod_With_A_Valid_QuestionnaireName_Then_The_Expected_FieldPeriod_Is_Returned()
        {
            // arrange
            var tests = new Dictionary<string, DateTime>()
            {
                { "FRS2404a", new DateTime(2024, 04, 1) },
                { "OPN2308a", new DateTime(2023, 08, 1) }
            };

            // act && assert
            foreach (var test in tests)
            {
                var fieldPeriod = QuestionnaireDtoMapper.GetFieldPeriod(test.Key);
                Assert.AreEqual(test.Value, fieldPeriod);
            }
        }

        [TestCase("FRSL404a")]
        [TestCase("OPNTESTa")]
        [TestCase("LMS20B")]
        public void Given_I_Call_GetFieldPeriod_With_A_Invalid_QuestionnaireName_Then_Null_Is_Returned(string questionnaireName)
        {
            // act
            var fieldPeriod = QuestionnaireDtoMapper.GetFieldPeriod(questionnaireName);

            // assert
            Assert.IsNull(fieldPeriod);
        }

        [TestCase("FRS2404a", "FRS")]
        [TestCase("OPN2308a", "OPN")]
        [TestCase("LMS2406_tl1", "LMS")]
        public void Given_I_Call_GetSurveyTla_With_A_Valid_QuestionnaireName_Then_The_Expected_FieldPeriod_Is_Returned(string questionnaireName, string expectedResult)
        {
            // act
            var surveyTla = QuestionnaireDtoMapper.GetSurveyTla(questionnaireName);

            // assert
            Assert.AreEqual(expectedResult, surveyTla);
        }

        [TestCase("F")]
        [TestCase("OP")]
        public void Given_I_Call_GetSurveyTla_With_A_Invalid_QuestionnaireName_Then_Null_Is_Returned(string questionnaireName)
        {
            // act
            var surveyTla = QuestionnaireDtoMapper.GetSurveyTla(questionnaireName);

            // assert
            Assert.IsNull(surveyTla);
        }
    }
}
