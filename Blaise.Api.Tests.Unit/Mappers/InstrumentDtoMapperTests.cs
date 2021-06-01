using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class InstrumentDtoMapperTests
    {
        private InstrumentDtoMapper _sut;
        private Mock<IInstrumentStatusMapper> _statusMapperMock;
        private Mock<IInstrumentNodeDtoMapper> _nodeDtoMapperMock;

        private DateTime _installDate;
        private int _numberOfRecordForInstrument;
        private Mock<ISurvey> _surveyMock;
        private Mock<ISurveyReportingInfo> _surveyReportingInfoMock;
        
        [SetUp]
        public void SetupTests()
        {
            _installDate = DateTime.Now;
            _numberOfRecordForInstrument = 100;

            _surveyMock = new Mock<ISurvey>();
            _surveyMock.Setup(s => s.InstallDate).Returns(_installDate);

            _surveyReportingInfoMock = new Mock<ISurveyReportingInfo>();
            _surveyReportingInfoMock.Setup(r => r.DataRecordCount).Returns(_numberOfRecordForInstrument);
            _surveyMock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(_surveyReportingInfoMock.Object);

            _statusMapperMock = new Mock<IInstrumentStatusMapper>();
            _nodeDtoMapperMock = new Mock<IInstrumentNodeDtoMapper>();

            _sut = new InstrumentDtoMapper(_statusMapperMock.Object, _nodeDtoMapperMock.Object);
        }

        [Test]
        public void Given_A_Survey_When_I_Call_MapToInstrumentDto_Then_Properties_Are_Mapped_Correctly()
        {
            //arrange
            const string instrument1Name = "OPN2010A";
            var instrument1Id = Guid.NewGuid();
            const string serverPark1Name = "ServerParkA";

            const int numberOfRecordForInstrument1 = 20;

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(instrument1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(instrument1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForInstrument1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);
            
            _statusMapperMock.Setup(s => s.GetInstrumentStatus(survey1Mock.Object))
                .Returns(SurveyStatusType.Active);

            var nodeList = new List<InstrumentNodeDto>
            {
                new InstrumentNodeDto()
            };

            _nodeDtoMapperMock.Setup(n => n.MapToInstrumentNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            //act
            var result = _sut.MapToInstrumentDto(survey1Mock.Object);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InstrumentDto>(result);
            Assert.AreEqual(instrument1Name, result.Name);
            Assert.AreEqual(instrument1Id, result.Id);
            Assert.AreEqual(serverPark1Name, result.ServerParkName);
            Assert.AreEqual(numberOfRecordForInstrument1, result.DataRecordCount);
            Assert.AreEqual(SurveyStatusType.Active.ToString(), result.Status);
            Assert.True(result.HasData);
            Assert.AreSame(nodeList, result.Nodes);
        }

        [Test]
        public void Given_A_List_Of_Surveys_When_I_Call_MapToInstrumentDtos_Then_The_General_Properties_Are_Mapped()
        {
            //arrange
            const string instrument1Name = "OPN2010A";
            const string instrument2Name = "OPN2010B";

            var instrument1Id = Guid.NewGuid();
            var instrument2Id = Guid.NewGuid();

            const string serverPark1Name = "ServerParkA";
            const string serverPark2Name = "ServerParkB";

            const int numberOfRecordForInstrument1 = 20;
            const int numberOfRecordForInstrument2 = 0;

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.As<ISurvey2>();
            survey1Mock.Setup(s => s.Name).Returns(instrument1Name);
            survey1Mock.Setup(s => s.InstrumentID).Returns(instrument1Id);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1Name);

            var surveyReportingInfoMock1 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock1.Setup(r => r.DataRecordCount).Returns(numberOfRecordForInstrument1);
            survey1Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock1.Object);

            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.As<ISurvey2>();
            survey2Mock.Setup(s => s.Name).Returns(instrument2Name);
            survey2Mock.Setup(s => s.InstrumentID).Returns(instrument2Id);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark2Name);

            var surveyReportingInfoMock2 = new Mock<ISurveyReportingInfo>();
            surveyReportingInfoMock2.Setup(r => r.DataRecordCount).Returns(numberOfRecordForInstrument2);
            survey2Mock.As<ISurvey2>().Setup(s => s.GetReportingInfo()).Returns(surveyReportingInfoMock2.Object);
            
            var surveys = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object
            };

            _statusMapperMock.Setup(s => s.GetInstrumentStatus(survey1Mock.Object))
                .Returns(SurveyStatusType.Active);

            _statusMapperMock.Setup(s => s.GetInstrumentStatus(survey2Mock.Object))
                .Returns(SurveyStatusType.Installing);

            var nodeList = new List<InstrumentNodeDto>
            {
                new InstrumentNodeDto(),
                new InstrumentNodeDto()
            };

            _nodeDtoMapperMock.Setup(n => n.MapToInstrumentNodeDtos(It.IsAny<IMachineConfigurationCollection>()))
                .Returns(nodeList);

            //act
            var result = _sut.MapToInstrumentDtos(surveys).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<InstrumentDto>>(result);
            Assert.AreEqual(2, result.Count);

            Assert.True(result.Any(i =>
                i.Name == instrument1Name &&
                i.Id == instrument1Id &&
                i.ServerParkName == serverPark1Name &&
                i.DataRecordCount == numberOfRecordForInstrument1 &&
                i.Status == SurveyStatusType.Active.ToString() &&
                i.HasData &&
                i.Nodes.Count() == 2));

            Assert.True(result.Any(i =>
                i.Name == instrument2Name &&
                i.Id == instrument2Id &&
                i.ServerParkName == serverPark2Name &&
                i.DataRecordCount == numberOfRecordForInstrument2 &&
                i.Status == SurveyStatusType.Installing.ToString() &&
                i.HasData == false &&
                i.Nodes.Count() == 2));
        }
    }
}
