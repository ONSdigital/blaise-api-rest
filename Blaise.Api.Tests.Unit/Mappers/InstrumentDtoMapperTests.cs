using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Extensions;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class InstrumentDtoMapperTests
    {
        private InstrumentDtoMapper _sut;
        private string _instrumentName;
        private string _serverParkName;
        private DateTime _installDate;
        private SurveyStatusType _surveyStatus;
        private int _numberOfRecordForInstrument;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;

        [SetUp]
        public void SetupTests()
        {
            _instrumentName = "OPN2010A";
            _serverParkName = "ServerParkA";
            _installDate = DateTime.Now;
            _surveyStatus = SurveyStatusType.Active;
            _numberOfRecordForInstrument = 100;

            _surveyMock = new Mock<ISurvey>();
            _surveyMock.Setup(s => s.Name).Returns(_instrumentName);
            _surveyMock.Setup(s => s.ServerPark).Returns(_serverParkName);
            _surveyMock.Setup(s => s.InstallDate).Returns(_installDate);
            _surveyMock.Setup(s => s.Status).Returns(_surveyStatus.FullName);

            _surveyReportingInfoMock = new Mock<ISurveyReportingInfo>();
            _surveyReportingInfoMock.Setup(r => r.DataRecordCount).Returns(_numberOfRecordForInstrument);
            _surveyMock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(_surveyReportingInfoMock.Object);

            _sut = new InstrumentDtoMapper();
        }

        [Test]
        public void Given_A_List_Of_Surveys_When_I_Call_MapToInstrumentDtos_Then_The_Correct_Properties_Are_Mapped()
        {
            //arrange
            const string instrument1Name = "OPN2010A";
            const string instrument2Name = "OPN2010B";

            const string serverPark1Name = "ServerParkA";
            const string serverPark2Name = "ServerParkB";

            const int numberOfRecordForInstrument1 = 20;
            const int numberOfRecordForInstrument2 = 0;

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(instrument1Name);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);
            survey1Mock.Setup(s => s.Status).Returns(SurveyStatusType.Active.FullName());

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForInstrument1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);

            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.As<ISurvey2>();
            survey2Mock.Setup(s => s.Name).Returns(instrument2Name);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark2Name);
            survey2Mock.Setup(s => s.Status).Returns(SurveyStatusType.Inactive.FullName());

            var surveyReportingInfoMock2 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock2.Setup(r => r.DataRecordCount).Returns(numberOfRecordForInstrument2);
            survey2Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock2.Object);

            var surveys = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object
            };

            //act
            var result = _sut.MapToInstrumentDtos(surveys).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(i =>
                i.Name == instrument1Name &&
                i.ServerParkName == serverPark1Name &&
                i.Status == SurveyStatusType.Active.FullName() &&
                i.DataRecordCount == numberOfRecordForInstrument1 &&
                i.HasData));

            Assert.True(result.Any(i =>
                i.Name == instrument2Name &&
                i.ServerParkName == serverPark2Name &&
                i.Status == SurveyStatusType.Inactive.FullName() &&
                i.DataRecordCount == numberOfRecordForInstrument2 &&
                i.HasData == false));
        }

        [TestCase(0, false, SurveyStatusType.Active)]
        [TestCase(0, false, SurveyStatusType.Inactive)]
        [TestCase(1, true, SurveyStatusType.Active)]
        [TestCase(1, true, SurveyStatusType.Inactive)]
        [TestCase(100, true, SurveyStatusType.Active)]
        [TestCase(100, true, SurveyStatusType.Inactive)]
        public void Given_A_Survey_When_I_Call_MapToInstrumentDto_Then_The_Correct_Properties_Are_Mapped(int numberOfRecords,
            bool hasData, SurveyStatusType surveyStatus)
        {
            //arrange
            _surveyMock.Setup(s => s.Status).Returns(surveyStatus.FullName);
            _surveyReportingInfoMock.Setup(s => s.DataRecordCount)
                .Returns(numberOfRecords);

            //act
            var result = _sut.MapToInstrumentDto(_surveyMock.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InstrumentDto>(result);
            Assert.AreEqual(_instrumentName, result.Name);
            Assert.AreEqual(_serverParkName, result.ServerParkName);
            Assert.AreEqual(_installDate, result.InstallDate);
            Assert.AreEqual(numberOfRecords, result.DataRecordCount);
            Assert.AreEqual(hasData, result.HasData);
            Assert.AreEqual(surveyStatus.FullName(), result.Status);
        }

        [Test]
        public void Given_An_Instrument_And_SurveyDays_When_I_Call_MapToCatiInstrumentDto_Then_A_CatiInstrumentDto_Is_Returned()
        {
            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, new List<DateTime>());

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
        }

        [TestCase(0, false, SurveyStatusType.Active)]
        [TestCase(0, false, SurveyStatusType.Inactive)]
        [TestCase(1, true, SurveyStatusType.Active)]
        [TestCase(1, true, SurveyStatusType.Inactive)]
        [TestCase(100, true, SurveyStatusType.Active)]
        [TestCase(100, true, SurveyStatusType.Inactive)]
        public void Given_A_Survey_When_I_Call_MapToCatiInstrumentDto_Then_The_Correct_Properties_Are_Mapped(int numberOfRecords,
            bool hasData, SurveyStatusType surveyStatus)
        {
            //arrange
            _surveyMock.Setup(s => s.Status).Returns(surveyStatus.FullName);
            _surveyReportingInfoMock.Setup(s => s.DataRecordCount)
                .Returns(numberOfRecords);

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, new List<DateTime>());

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.AreEqual(_instrumentName, result.Name);
            Assert.AreEqual(_serverParkName, result.ServerParkName);
            Assert.AreEqual(_installDate, result.InstallDate);
            Assert.AreEqual(numberOfRecords, result.DataRecordCount);
            Assert.AreEqual(hasData, result.HasData);
            Assert.AreEqual(surveyStatus.FullName(), result.Status);
        }

        [Test]
        public void Given_No_Survey_Days_When_I_Call_MapToCatiInstrumentDto_Then_The_Instrument_Is_Not_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>();

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_All_SurveyDays_Are_In_The_Future_When_I_Call_MapToInstrumentDto_Then_The_Instrument_Is_Not_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_SurveyDays_Have_All_Passed_When_I_Call_MapToInstrumentDto_Then_The_Instrument_Is_Not_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.Active);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_In_The_Future_When_I_Call_MapToInstrumentDto_Then_The_Instrument_Is_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_When_I_Call_MapToInstrumentDto_Then_The_Instrument_Is_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToInstrumentDto_Then_The_Instrument_Is_Active()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public void Given_No_Survey_For_Today_When_I_Call_MapToInstrumentDto_Then_The_ActiveToday_Field_Is_Marked_As_False()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.ActiveToday);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_For_Today_When_I_Call_MapToInstrumentDto_Then_The_ActiveToday_Field_Is_Marked_As_True()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today,
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.ActiveToday);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToInstrumentDto_Then_The_ActiveToday_Field_Is_Marked_As_True()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.ActiveToday);
        }

        [Test]
        public void Given_No_Survey_Days_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_False()
        {
            //arrange
            var surveyDays = new List<DateTime>();

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.DeliverData);
        }

        [Test]
        public void Given_All_SurveyDays_Are_In_The_Future_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_False()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.DeliverData);
        }

        [Test]
        public void Given_There_Is_A_SurveyDay_In_The_Future_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_True()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_True()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_There_A_SurveyDay_For_Today_At_A_Later_Time_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_True()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddHours(1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_The_Last_SurveyDay_Is_Yesterday_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_True_Due_To_DataDelivery_Requirements()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(-1)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsTrue(result.DeliverData);
        }

        [Test]
        public void Given_The_Last_SurveyDay_Is_Two_Days_Ago_Or_More_When_I_Call_MapToCatiInstrumentDto_Then_DeliverData_Is_False_Due_To_DataDelivery_Requirements()
        {
            //arrange
            var surveyDays = new List<DateTime>
            {
                DateTime.Today.AddDays(3),
                DateTime.Today.AddDays(2)
            };

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.IsFalse(result.DeliverData);
        }
    }
}
