using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class CatiInstrumentDtoMapperTests
    {
        private CatiInstrumentDtoMapper _sut;
        private Mock<IInstrumentStatusMapper> _statusMapperMock;
        private Mock<IInstrumentNodeDtoMapper> _nodeDtoMapperMock;

        private string _instrumentName;
        private string _serverParkName;
        private DateTime _installDate;
        private int _numberOfRecordForInstrument;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;
        
        [SetUp]
        public void SetupTests()
        {
            _instrumentName = "OPN2010A";
            _serverParkName = "ServerParkA";
            _installDate = DateTime.Now;
            _numberOfRecordForInstrument = 100;

            _surveyMock = new Mock<ISurvey>();
            _surveyMock.Setup(s => s.Name).Returns(_instrumentName);
            _surveyMock.Setup(s => s.ServerPark).Returns(_serverParkName);
            _surveyMock.Setup(s => s.InstallDate).Returns(_installDate);

            _surveyReportingInfoMock = new Mock<ISurveyReportingInfo>();
            _surveyReportingInfoMock.Setup(r => r.DataRecordCount).Returns(_numberOfRecordForInstrument);
            _surveyMock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(_surveyReportingInfoMock.Object);

            _statusMapperMock = new Mock<IInstrumentStatusMapper>();
            _nodeDtoMapperMock = new Mock<IInstrumentNodeDtoMapper>();

            _sut = new CatiInstrumentDtoMapper(_statusMapperMock.Object, _nodeDtoMapperMock.Object);
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

        [TestCase(20, true)]
        [TestCase(1, true)]
        [TestCase(0, false)]
        public void Given_A_Survey_When_I_Call_MapToCatiInstrumentDto_Then_Properties_Are_Mapped_Correctly(int numberOfRecords,
            bool hasData)
        {
            //arrange
            _surveyReportingInfoMock.Setup(s => s.DataRecordCount)
                .Returns(numberOfRecords);

            _statusMapperMock.Setup(s => s.GetInstrumentStatus(_surveyMock.Object))
                .Returns(SurveyStatusType.Active);

            var nodeList = new List<InstrumentNodeDto>
            {
                new InstrumentNodeDto()
            };

            _nodeDtoMapperMock.Setup(n => n.MapToInstrumentNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            //act
            var result = _sut.MapToCatiInstrumentDto(_surveyMock.Object, new List<DateTime>());

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiInstrumentDto>(result);
            Assert.AreEqual(_instrumentName, result.Name);
            Assert.AreEqual(_serverParkName, result.ServerParkName);
            Assert.AreEqual(_installDate, result.InstallDate);
            Assert.AreEqual(numberOfRecords, result.DataRecordCount);
            Assert.AreEqual(SurveyStatusType.Active.ToString(), result.Status);
            Assert.AreEqual(hasData, result.HasData);
            Assert.AreSame(nodeList, result.Nodes);
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
