using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Mappers;
using Blaise.Api.Tests.Unit.Helpers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class CaseDtoMapperTests
    {
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private CaseDtoMapper _sut;

        [SetUp]
        public void SetUpTests()
        {
            // Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _sut = new CaseDtoMapper(_blaiseCaseApiMock.Object);
        }

        [Test]
        public void Given_A_List_Of_CaseStatusModels_When_I_Call_GetCaseStatusList_Then_I_Get_A_List_Containing_Two_CaseStatusDtos_Back()
        {
            // arrange
            var caseStatusModelList = new List<CaseStatusModel>
            {
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000007"), 110, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000008"), 210, DateTime.Today.ToString(CultureInfo.InvariantCulture))
            };

            // act
            var result = _sut.MapToCaseStatusDtoList(caseStatusModelList);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsInstanceOf<IEnumerable<CaseStatusDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(r => r.PrimaryKey == "0000007" && r.Outcome == 110));
            Assert.IsTrue(result.Any(r => r.PrimaryKey == "0000008" && r.Outcome == 210));
        }

        [Test]
        public void Given_An_Empty_List_Of_CaseStatusModels_When_I_Call_GetCaseStatusList_Then_I_Get_An_Empty_List_Back()
        {
            // arrange
            // act
            var result = _sut.MapToCaseStatusDtoList(new List<CaseStatusModel>());

            // assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.IsInstanceOf<IEnumerable<CaseStatusDto>>(result);
        }

        [Test]
        public void Given_A_Valid_CaseRecord_When_I_Call_MapToCaseDto_Then_The_Correct_CaseDto_Is_Returned()
        {
            // arrange
            var dataRecordMock = new Mock<IDataRecord>();

            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(dataRecordMock.Object)).Returns(fieldData);

            // act
            var result = _sut.MapToCaseDto(caseId, dataRecordMock.Object);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CaseDto>(result);
            Assert.AreEqual(caseId, result.CaseId);
            Assert.AreEqual(fieldData, result.FieldData);
        }

        [Test]
        public void Given_Multikey_Arguments_And_A_Valid_CaseRecord_When_I_Call_MapToCaseMultikeyDto_Then_A_Correct_Case_Multikey_Dto_Is_Returned()
        {
            // arrange
            var dataRecordMock = new Mock<IDataRecord>();
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" }
            };

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(dataRecordMock.Object)).Returns(fieldData);

            // act
            var result = _sut.MapToCaseMultikeyDto(primaryKeyValues, dataRecordMock.Object);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CaseMultikeyDto>(result);
            Assert.AreEqual(primaryKeyValues, result.PrimaryKeyValues);
            Assert.AreEqual(fieldData, result.FieldData);
        }

        [Test]
        public void Given_A_Valid_CaseRecord_When_I_Call_MapToCaseEditInformationDto_Then_A_Correct_CaseEditInformationDto_Is_Returned()
        {
            // Arrange
            var primaryKey = "10001011";
            var outcome = 110;
            var assignedTo = "Dr Doom";
            var interviewer = "Mr fantastic";
            var editedStatus = 2;
            var organisation = 1;

            var dataRecordMock = new Mock<IDataRecord>();

            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QID.Serial_Number").ValueAsText).Returns(primaryKey);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QHAdmin.HOut").IntegerValue).Returns(outcome);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QEdit.AssignedTo").ValueAsText).Returns(assignedTo);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QHAdmin.Interviewer[1]").ValueAsText).Returns(interviewer);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QEdit.EditedStatus").EnumerationValue).Returns((int)editedStatus);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "orgID").EnumerationValue).Returns((int)organisation);

            // Act
            var result = _sut.MapToCaseEditInformationDto(dataRecordMock.Object);

            // Assert
            Assert.AreEqual(primaryKey, result.PrimaryKey);
            Assert.AreEqual(outcome, result.Outcome);
            Assert.AreEqual(assignedTo, result.AssignedTo);
            Assert.AreEqual(interviewer, result.Interviewer);
            Assert.AreEqual(editedStatus, result.EditedStatus);
            Assert.AreEqual(organisation, result.Organisation);
        }

        [Test]
        public void Given_A_Null_CaseRecord_When_I_Call_MapToCaseEditInformationDto_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            // act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.MapToCaseEditInformationDto(null));

            // assert
            Assert.AreEqual("The argument 'caseRecord' must be supplied", exception?.ParamName);
        }
    }
}
