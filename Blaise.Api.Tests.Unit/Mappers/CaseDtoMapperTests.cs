namespace Blaise.Api.Tests.Unit.Mappers
{
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

    public class CaseDtoMapperTests
    {
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private CaseDtoMapper _sut;

        [SetUp]
        public void SetUpTests()
        {
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
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000008"), 210, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
            };

            // act
            var result = _sut.MapToCaseStatusDtoList(caseStatusModelList);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf<IEnumerable<CaseStatusDto>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(r => r.PrimaryKey == "0000007" && r.Outcome == 110), Is.True);
            Assert.That(result.Any(r => r.PrimaryKey == "0000008" && r.Outcome == 210), Is.True);
        }

        [Test]
        public void Given_An_Empty_List_Of_CaseStatusModels_When_I_Call_GetCaseStatusList_Then_I_Get_An_Empty_List_Back()
        {
            // arrange
            var emptyList = new List<CaseStatusModel>();

            // act
            var result = _sut.MapToCaseStatusDtoList(emptyList);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Is.InstanceOf<IEnumerable<CaseStatusDto>>());
        }

        [Test]
        public void Given_A_Valid_CaseRecord_When_I_Call_MapToCaseDto_Then_The_Correct_CaseDto_Is_Returned()
        {
            // arrange
            var dataRecordMock = new Mock<IDataRecord>();

            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(dataRecordMock.Object)).Returns(fieldData);

            // act
            var result = _sut.MapToCaseDto(CaseId, dataRecordMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<CaseDto>());
            Assert.That(result.CaseId, Is.EqualTo(CaseId));
            Assert.That(result.FieldData, Is.EqualTo(fieldData));
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
                { "id", "9000001" },
            };

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(dataRecordMock.Object)).Returns(fieldData);

            // act
            var result = _sut.MapToCaseMultikeyDto(primaryKeyValues, dataRecordMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<CaseMultikeyDto>());
            Assert.That(result.PrimaryKeyValues, Is.EqualTo(primaryKeyValues));
            Assert.That(result.FieldData, Is.EqualTo(fieldData));
        }

        [Test]
        public void Given_A_Valid_CaseRecord_When_I_Call_MapToCaseEditInformationDto_Then_A_Correct_CaseEditInformationDto_Is_Returned()
        {
            // arrange
            const string PrimaryKey = "10001011";
            const int Outcome = 110;
            const string AssignedTo = "Dr Doom";
            const string Interviewer = "Mr Fantastic";
            const int EditedStatus = 2;
            const int Organisation = 1;

            var dataRecordMock = new Mock<IDataRecord>();

            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QID.Serial_Number").ValueAsText).Returns(PrimaryKey);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QHAdmin.HOut").IntegerValue).Returns(Outcome);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QEdit.AssignedTo").ValueAsText).Returns(AssignedTo);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QHAdmin.Interviewer[1]").ValueAsText).Returns(Interviewer);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "QEdit.EditedStatus").EnumerationValue).Returns(EditedStatus);
            _blaiseCaseApiMock.Setup(c => c.GetFieldValue(dataRecordMock.Object, "orgID").EnumerationValue).Returns(Organisation);

            // act
            var result = _sut.MapToCaseEditInformationDto(dataRecordMock.Object);

            // assert
            Assert.That(result.PrimaryKey, Is.EqualTo(PrimaryKey));
            Assert.That(result.Outcome, Is.EqualTo(Outcome));
            Assert.That(result.AssignedTo, Is.EqualTo(AssignedTo));
            Assert.That(result.Interviewer, Is.EqualTo(Interviewer));
            Assert.That(result.EditedStatus, Is.EqualTo(EditedStatus));
            Assert.That(result.Organisation, Is.EqualTo(Organisation));
        }

        [Test]
        public void Given_A_Null_CaseRecord_When_I_Call_MapToCaseEditInformationDto_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            IDataRecord caseRecord = null;

            // act
            TestDelegate act = () => _sut.MapToCaseEditInformationDto(caseRecord);

            // assert
            var exception = Assert.Throws<ArgumentNullException>(act);
            Assert.That("The argument 'caseRecord' must be supplied", Is.EqualTo(exception?.ParamName));
        }
    }
}
