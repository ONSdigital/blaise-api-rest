namespace Blaise.Api.Tests.Unit.Mappers
{
    using System;
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Cati;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Mappers;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Blaise.Nuget.Api.Contracts.Models;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class CatiDtoMapperTests
    {
        private CatiDtoMapper _sut;
        private Mock<IQuestionnaireStatusMapper> _statusMapperMock;
        private Mock<IQuestionnaireNodeDtoMapper> _nodeDtoMapperMock;

        private string _questionnaireName;
        private Guid _questionnaireId;
        private string _serverParkName;
        private DateTime _installDate;
        private int _numberOfRecordForQuestionnaire;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;

        [SetUp]
        public void SetupTests()
        {
            _questionnaireName = "OPN2010A";
            _questionnaireId = Guid.NewGuid();
            _serverParkName = "ServerParkA";
            _installDate = DateTime.Now;
            _numberOfRecordForQuestionnaire = 100;

            _surveyMock = new Mock<ISurvey>();
            _surveyMock.Setup(s => s.Name).Returns(_questionnaireName);
            _surveyMock.Setup(s => s.InstrumentID).Returns(_questionnaireId);
            _surveyMock.Setup(s => s.ServerPark).Returns(_serverParkName);
            _surveyMock.Setup(s => s.InstallDate).Returns(_installDate);

            _surveyReportingInfoMock = new Mock<ISurveyReportingInfo>();
            _surveyReportingInfoMock.Setup(r => r.DataRecordCount).Returns(_numberOfRecordForQuestionnaire);
            _surveyMock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(_surveyReportingInfoMock.Object);

            _statusMapperMock = new Mock<IQuestionnaireStatusMapper>();
            _nodeDtoMapperMock = new Mock<IQuestionnaireNodeDtoMapper>();

            _sut = new CatiDtoMapper(_statusMapperMock.Object, _nodeDtoMapperMock.Object);
        }

        [Test]
        public void Given_A_Questionnaire_And_SurveyDays_When_I_Call_MapToCatiQuestionnaireDto_Then_A_CatiQuestionnaireDto_Is_Returned()
        {
            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, new List<DateTime>());

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
        }

        [TestCase(20, true)]
        [TestCase(1, true)]
        [TestCase(0, false)]
        public void Given_A_Survey_When_I_Call_MapToCatiQuestionnaireDto_Then_Properties_Are_Mapped_Correctly(
            int numberOfRecords,
            bool hasData)
        {
            // arrange
            _surveyReportingInfoMock.Setup(s => s.DataRecordCount)
                .Returns(numberOfRecords);

            _statusMapperMock.Setup(s => s.GetQuestionnaireStatus(_surveyMock.Object))
                .Returns(QuestionnaireStatusType.Active);

            var nodeList = new List<QuestionnaireNodeDto>
            {
                new QuestionnaireNodeDto(),
            };

            _nodeDtoMapperMock.Setup(n => n.MapToQuestionnaireNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, new List<DateTime>());

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.AreEqual(_questionnaireName, result.Name);
            Assert.AreEqual(_questionnaireId, result.Id);
            Assert.AreEqual(_serverParkName, result.ServerParkName);
            Assert.AreEqual(_installDate, result.InstallDate);
            Assert.AreEqual(numberOfRecords, result.DataRecordCount);
            Assert.AreEqual(QuestionnaireStatusType.Active.ToString(), result.Status);
            Assert.AreEqual(hasData, result.HasData);
            Assert.AreSame(nodeList, result.Nodes);
        }

        [Test]
        public void Given_No_Survey_Days_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Not_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>();

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_All_SurveyDays_Are_In_The_Future_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Not_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_SurveyDays_Have_All_Passed_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Not_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_In_The_Future_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToCatiQuestionnaireDto_Then_The_Questionnaire_Is_Active()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_No_Survey_For_Today_When_I_Call_MapToCatiQuestionnaireDto_Then_The_ActiveToday_Field_Is_Marked_As_False()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.ActiveToday);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_For_Today_When_I_Call_MapToCatiQuestionnaireDto_Then_The_ActiveToday_Field_Is_Marked_As_True()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today,
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.ActiveToday);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToCatiQuestionnaireDto_Then_The_ActiveToday_Field_Is_Marked_As_True()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.ActiveToday);
        }

        [Test]
        public void Given_No_Survey_Days_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_False()
        {
            // arrange
            var surveyDays = new List<DateTime>();

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.DeliverData);
        }

        [Test]
        public void Given_All_SurveyDays_Are_In_The_Future_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_False()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.DeliverData);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_In_The_Future_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_True()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_True()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_True()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_The_Last_SurveyDay_Is_Yesterday_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_True_Due_To_DataDelivery_Requirements()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-1),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_The_Last_SurveyDay_Is_Two_Days_Ago_Or_More_When_I_Call_MapToCatiQuestionnaireDto_Then_DeliverData_Is_False_Due_To_DataDelivery_Requirements()
        {
            // arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2),
            };

            // act
            var result = _sut.MapToCatiQuestionnaireDto(_surveyMock.Object, surveyDays);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.IsFalse(result.DeliverData);
        }

        [Test]
        public void Given_An_DayBatchModel_When_I_Call_MapToDayBatchDto_Then_A_DayBatchDto_Is_Returned()
        {
            // arrange
            var dayBatchModel = new DayBatchModel(DateTime.Today, new List<string>());

            // act
            var result = _sut.MapToDayBatchDto(dayBatchModel);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DayBatchDto>(result);
        }

        [Test]
        public void Given_An_DayBatchModel_When_I_Call_MapToDayBatchDto_Then_An_ExpectedDayBatchDto_Is_Returned()
        {
            // arrange
            var dayBatchDate = DateTime.Today;
            var caseIds = new List<string> { "90001", "90002" };
            var dayBatchModel = new DayBatchModel(dayBatchDate, caseIds);

            // act
            var result = _sut.MapToDayBatchDto(dayBatchModel);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DayBatchDto>(result);
            Assert.AreEqual(dayBatchDate, result.DayBatchDate);
            Assert.AreEqual(caseIds, result.CaseIds);
        }
    }
}
