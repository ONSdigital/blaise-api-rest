using Blaise.Api.Contracts.Enums;
using Blaise.Api.Contracts.Models.Edit;
using Blaise.Api.Core.Mappers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class EditingDtoMapperTests
    {
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private IBlaiseCaseApi blaiseCaseApi;
        private EditingDtoMapper _sut;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _sut = new EditingDtoMapper(_blaiseCaseApiMock.Object);
        }

        [TestCase(0, EditedStatusType.NotStarted)]
        [TestCase(1, EditedStatusType.Started)]
        [TestCase(2, EditedStatusType.Query)]
        [TestCase(3, EditedStatusType.Finished)]
        public void Given_A_Valid_CaseRecord_When_I_Call_EditingDtoMapper_Then_A_Correct_EditingDetailsDto_Is_Returned(int editedStatus, EditedStatusType editedStatusType)
        {
            //Arrange
            var caseRecord = new Mock<IDataRecord>();

            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(caseRecord.Object, "QID.Serial_Number").ValueAsText).Returns("10001011");
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(caseRecord.Object, "Admin.HOut").IntegerValue).Returns(110);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(caseRecord.Object, "QEdit.AssignedTo").ValueAsText).Returns("Dr Doom");
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(caseRecord.Object, "QEdit.EditedStatus").IntegerValue).Returns(editedStatus);

            var expectedEditingDetailsDto = new EditingDetailsDto
            {
                PrimaryKey = "10001011",
                Outcome = 110,
                AssignedTo = "Dr Doom",
                EditedStatus = editedStatusType,
                // TODO
                Interviewer = "",
            };

            //Act
            var editingDetailsDto = _sut.MapToEditingDetailsDto(caseRecord.Object);

            //Assert
            Assert.AreEqual(expectedEditingDetailsDto.PrimaryKey, editingDetailsDto.PrimaryKey);
            Assert.AreEqual(expectedEditingDetailsDto.Outcome, editingDetailsDto.Outcome);
            Assert.AreEqual(expectedEditingDetailsDto.AssignedTo, editingDetailsDto.AssignedTo);
            Assert.AreEqual(expectedEditingDetailsDto.EditedStatus, editingDetailsDto.EditedStatus);
            Assert.AreEqual(expectedEditingDetailsDto.Interviewer, editingDetailsDto.Interviewer);
        }

        [Test]
        public void Given_A_Null_CaseRecord_When_I_Call_EditingDtoMapper_Then_A_ArgumentNullException_Is_Thrown()
        {
            //arrange
            //act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.MapToEditingDetailsDto(null));

            //assert
            Assert.AreEqual("The argument 'caseRecord' must be supplied", exception.ParamName);
        }
    }
}
