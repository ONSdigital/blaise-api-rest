using Blaise.Api.Contracts.Models.Edit;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;
using System;
using System.Collections.Generic;

namespace Blaise.Api.Tests.Unit.Services
{
    public class EditingServiceTests
    {
        private IEditingService _sut;

        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<IDataRecord> _dataRecordMock;
        private Mock<IBlaiseSqlApi> _blaiseSqlApiMock;
        private Mock<IEditingDtoMapper> _editingDtoMapperMock;

        private string _questionnaireName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _dataRecordMock = new Mock<IDataRecord>();
            _blaiseSqlApiMock = new Mock<IBlaiseSqlApi>();
            _editingDtoMapperMock = new Mock<IEditingDtoMapper>();

            _serverParkName = "LocalDevelopment";
            _questionnaireName = "FRS2504A";

            _sut = new EditingService(_blaiseCaseApiMock.Object, _blaiseSqlApiMock.Object, _editingDtoMapperMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditingDetailsList_Then_I_Get_A_List_Of_CaseEditingDetailsDto_Back()
        {
            //arrange
            //act
            var result = _sut.GetCaseEditingDetailsList(_serverParkName, _questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<EditingDetailsDto>>(result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditingDetailsList_Then_The_Correct_Services_Are_Called()
        {
            //arrange
            var casesIds = new List<string>
            {
                "10001011",
                "10001012",
                "10001013"
            };

            var case1Mock = new Mock<IDataRecord>();
            var case2Mock = new Mock<IDataRecord>();
            var case3Mock = new Mock<IDataRecord>();

            _blaiseSqlApiMock.Setup(z => z.GetCaseIds(_questionnaireName)).Returns(casesIds);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } }, _questionnaireName, _serverParkName)).Returns(case1Mock.Object);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001012" } }, _questionnaireName, _serverParkName)).Returns(case2Mock.Object);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001013" } }, _questionnaireName, _serverParkName)).Returns(case3Mock.Object);

            //act
            _sut.GetCaseEditingDetailsList(_serverParkName, _questionnaireName);

            //assert                
            _blaiseSqlApiMock.Verify(b => b.GetCaseIds(_questionnaireName), Times.Once);
            _blaiseCaseApiMock.Verify(b => b.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } }, _questionnaireName, _serverParkName),Times.Once);
            _blaiseCaseApiMock.Verify(b => b.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001012" } }, _questionnaireName, _serverParkName), Times.Once);
            _blaiseCaseApiMock.Verify(b => b.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001013" } }, _questionnaireName, _serverParkName), Times.Once);
            _editingDtoMapperMock.Verify(b => b.MapToEditingDetailsDto(case1Mock.Object), Times.Once);
            _editingDtoMapperMock.Verify(b => b.MapToEditingDetailsDto(case2Mock.Object), Times.Once);
            _editingDtoMapperMock.Verify(b => b.MapToEditingDetailsDto(case3Mock.Object), Times.Once);
        }
        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditingDetailsList_Then_An_Expected_List_Of_EditingDetailsDto_Are_Returned()
        {
            //arrange
            var casesIds = new List<string>
            {
                "10001011",
                "10001012",
                "10001013"
            };

            var case1Mock = new Mock<IDataRecord>();
            var case2Mock = new Mock<IDataRecord>();
            var case3Mock = new Mock<IDataRecord>();

            _blaiseSqlApiMock.Setup(z => z.GetCaseIds(_questionnaireName)).Returns(casesIds);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } }, _questionnaireName, _serverParkName)).Returns(case1Mock.Object);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001012" } }, _questionnaireName, _serverParkName)).Returns(case2Mock.Object);
            _blaiseCaseApiMock.Setup(z => z.GetCase(new Dictionary<string, string> { { "QID.Serial_Number", "10001013" } }, _questionnaireName, _serverParkName)).Returns(case3Mock.Object);

            var editingDetailsDto1 = new EditingDetailsDto();
            var editingDetailsDto2 = new EditingDetailsDto();
            var editingDetailsDto3 = new EditingDetailsDto();

            _editingDtoMapperMock.Setup(z => z.MapToEditingDetailsDto(case1Mock.Object)).Returns(editingDetailsDto1);
            _editingDtoMapperMock.Setup(z => z.MapToEditingDetailsDto(case2Mock.Object)).Returns(editingDetailsDto2);
            _editingDtoMapperMock.Setup(z => z.MapToEditingDetailsDto(case3Mock.Object)).Returns(editingDetailsDto3);

            //act
            var result = _sut.GetCaseEditingDetailsList(_serverParkName, _questionnaireName);

            //assert                
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(new List<EditingDetailsDto> { editingDetailsDto1 , editingDetailsDto2 , editingDetailsDto3 }));
        }
        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCaseEditingDetailsList_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            //act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCaseEditingDetailsList(_serverParkName, null));

            //assert
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCaseEditingDetailsList_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            //act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCaseEditingDetailsList(null, _questionnaireName));

            //assert
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCaseEditingDetailsList_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            //act
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCaseEditingDetailsList(_serverParkName, ""));

            //assert
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCaseEditingDetailsList_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            //act
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCaseEditingDetailsList("", _questionnaireName));

            //assert
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }
    }
}
