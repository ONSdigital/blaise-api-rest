namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Blaise.Api.Contracts.Models.Case;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Api.Core.Services;
    using Blaise.Api.Tests.Unit.Helpers;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Blaise.Nuget.Api.Contracts.Models;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.DataLink;
    using StatNeth.Blaise.API.DataRecord;

    public class CaseServiceTests
    {
        private const string QuestionnaireName = "OPN2101A";
        private const string ServerParkName = "LocalDevelopment";

        private ICaseService _sut;
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<ICaseDtoMapper> _mapperMock;
        private Mock<IBlaiseSqlApi> _blaiseSqlApiMock;
        private Mock<IDataRecord> _dataRecordMock;
        private Mock<IDataSet> _dataSetMock;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _mapperMock = new Mock<ICaseDtoMapper>();
            _blaiseSqlApiMock = new Mock<IBlaiseSqlApi>();
            _dataRecordMock = new Mock<IDataRecord>();
            _dataSetMock = new Mock<IDataSet>();

            _sut = new CaseService(_blaiseCaseApiMock.Object, _mapperMock.Object, _blaiseSqlApiMock.Object);
        }

        [Test]
        public void Given_A_Questionnaire_Has_Two_Cases_When_I_Call_GetCaseIds_Then_I_Get_A_List_Containing_Two_CaseIds_Back()
        {
            // arrange
            var caseStatusModels = new List<CaseStatusModel>
            {
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000007"), 110, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000008"), 210, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
            };

            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(QuestionnaireName, ServerParkName))
                .Returns(caseStatusModels);

            // act
            var result = _sut.GetCaseIds(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf<IEnumerable<string>>());
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Has.Member("0000007"));
            Assert.That(result, Has.Member("0000008"));
        }

        [Test]
        public void Given_A_Questionnaire_Has_No_Cases_When_I_Call_GetCaseIds_Then_I_Get_An_Empty_List_Back()
        {
            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(QuestionnaireName, ServerParkName))
                .Returns(new List<CaseStatusModel>());

            // act
            var result = _sut.GetCaseIds(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Is.InstanceOf<IEnumerable<string>>());
        }

        [Test]
        public void Given_A_Questionnaire_Has_Two_Cases_When_I_Call_GetCaseStatusList_Then_The_Expected_List_Of_CaseStatusDtos_Are_Returned()
        {
            // arrange
            var caseStatusModels = new List<CaseStatusModel> { new CaseStatusModel(), new CaseStatusModel() };
            var caseStatusDtos = new List<CaseStatusDto> { new CaseStatusDto(), new CaseStatusDto() };

            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(QuestionnaireName, ServerParkName))
                .Returns(caseStatusModels);

            _mapperMock.Setup(m => m.MapToCaseStatusDtoList(caseStatusModels)).Returns(caseStatusDtos);

            // act
            var result = _sut.GetCaseStatusList(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.SameAs(caseStatusDtos));
        }

        [Test]
        public void Given_A_Questionnaire_Has_Two_Cases_When_I_Call_GetCaseStatusList_Then_It_Calls_The_Expected_Services()
        {
            // arrange
            var caseStatusModels = new List<CaseStatusModel> { new CaseStatusModel(), new CaseStatusModel() };
            var caseStatusDtos = new List<CaseStatusDto> { new CaseStatusDto(), new CaseStatusDto() };

            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(QuestionnaireName, ServerParkName))
                .Returns(caseStatusModels);

            _mapperMock.Setup(m => m.MapToCaseStatusDtoList(caseStatusModels)).Returns(caseStatusDtos);

            // act
            _sut.GetCaseStatusList(ServerParkName, QuestionnaireName);

            // assert
            _blaiseCaseApiMock.Verify(v => v.GetCaseStatusModelList(QuestionnaireName, ServerParkName), Times.Once);
            _mapperMock.Verify(v => v.MapToCaseStatusDtoList(caseStatusModels), Times.Once);
        }

        [Test]
        public void Given_A_Questionnaire_Has_No_Cases_When_I_Call_GetCaseStatusList_Then_I_Get_An_Empty_List_Back()
        {
            // arrange
            var caseStatusModels = new List<CaseStatusModel>();
            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(QuestionnaireName, ServerParkName))
                .Returns(caseStatusModels);

            _mapperMock.Setup(m => m.MapToCaseStatusDtoList(caseStatusModels)).Returns(new List<CaseStatusDto>());

            // act
            var result = _sut.GetCaseStatusList(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Is.InstanceOf<IEnumerable<CaseStatusDto>>());
        }

        [Test]
        public void Given_I_Have_A_Case_With_A_PostCode_Set_When_I_Call_GetPostCode_Then_I_Get_The_PostCode_Back()
        {
            // arrange
            const string PostCode = "NP1 0AA";
            var dataValueMock = new Mock<IDataValue>();
            dataValueMock.Setup(dv => dv.ValueAsText).Returns(PostCode);

            const string CaseId = "0000007";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            _blaiseCaseApiMock.Setup(b => b.GetCase(primaryKey, QuestionnaireName, ServerParkName)).Returns(_dataRecordMock.Object);
            _blaiseCaseApiMock.Setup(f => f.GetFieldValue(_dataRecordMock.Object, FieldNameType.PostCode)).Returns(dataValueMock.Object);

            // act
            var result = _sut.GetPostCode(ServerParkName, QuestionnaireName, CaseId);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(PostCode));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKey, QuestionnaireName, ServerParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetPrimaryKeyValues(_dataRecordMock.Object)).Returns(primaryKey);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            // act
            _sut.GetCase(ServerParkName, QuestionnaireName, CaseId);

            // assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKey, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Expected_CaseDto_Is_Returned()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var caseDto = new CaseDto();

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKey, QuestionnaireName, ServerParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);
            _mapperMock.Setup(m => m.MapToCaseDto(CaseId, _dataRecordMock.Object)).Returns(caseDto);

            // act
            var result = _sut.GetCase(ServerParkName, QuestionnaireName, CaseId);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(caseDto));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Expected_Services_Are_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            var caseDto = new CaseDto();

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKey, QuestionnaireName, ServerParkName))
                .Returns(_dataRecordMock.Object);

            _mapperMock.Setup(m => m.MapToCaseDto(CaseId, _dataRecordMock.Object)).Returns(caseDto);

            // act
            _sut.GetCase(ServerParkName, QuestionnaireName, CaseId);

            // assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKey, QuestionnaireName, ServerParkName), Times.Once);
            _mapperMock.Verify(v => v.MapToCaseDto(CaseId, _dataRecordMock.Object), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                string.Empty,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                null,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                ServerParkName,
                string.Empty,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                ServerParkName,
                null,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseId' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("caseId"));
        }

        [Test]
        public void Given_Multikey_Arguments_When_I_Call_GetCase_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            var primaryKeys = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            // act
            _sut.GetCase(ServerParkName, QuestionnaireName, keyNames, keyValues);

            // assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKeys, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_Multikey_Arguments_When_I_Call_GetCase_Then_An_Expected_Case_Multikey_Dto_Is_Returned()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            var multiKeyDto = new CaseMultikeyDto
            {
                PrimaryKeyValues = primaryKeyValues,
                FieldData = fieldData,
            };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKeyValues, QuestionnaireName, ServerParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            _mapperMock.Setup(m => m.MapToCaseMultikeyDto(primaryKeyValues, _dataRecordMock.Object))
                .Returns(multiKeyDto);

            // act
            var result = _sut.GetCase(ServerParkName, QuestionnaireName, keyNames, keyValues);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(multiKeyDto));
        }

        [Test]
        public void Given_Multikey_Arguments_When_I_Call_GetCase_Then_The_Correct_services_Are_Called()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            var multiKeyDto = new CaseMultikeyDto
            {
                PrimaryKeyValues = primaryKeyValues,
                FieldData = fieldData,
            };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKeyValues, QuestionnaireName, ServerParkName))
                .Returns(_dataRecordMock.Object);

            _mapperMock.Setup(m => m.MapToCaseMultikeyDto(primaryKeyValues, _dataRecordMock.Object))
                .Returns(multiKeyDto);

            // act
            _sut.GetCase(ServerParkName, QuestionnaireName, keyNames, keyValues);

            // assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKeyValues, QuestionnaireName, ServerParkName), Times.Once);
            _mapperMock.Verify(v => v.MapToCaseMultikeyDto(primaryKeyValues, _dataRecordMock.Object), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCase_With_a_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                string.Empty,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCase__With_a_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                null,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                ServerParkName,
                string.Empty,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                ServerParkName,
                null,
                keyNames,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_keyNames_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyNames' must be supplied"));
        }

        [Test]
        public void Given_A_Null_keyNames_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                null,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("keyNames"));
        }

        [Test]
        public void Given_An_Empty_keyValues_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };
            var keyValues = new List<string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyValues' must be supplied"));
        }

        [Test]
        public void Given_A_Null_keyValues_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("keyValues"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCase_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act
            _sut.CreateCase(ServerParkName, QuestionnaireName, CaseId, fieldData);

            // assert
            _blaiseCaseApiMock.Verify(v => v.CreateCase(primaryKey, fieldData, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                string.Empty,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                null,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                string.Empty,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                null,
                CaseId,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                string.Empty,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseId' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                null,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("caseId"));
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'fieldData' must be supplied"));
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                CaseId,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("fieldData"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCase_With_A_Multikey_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            // act
            _sut.CreateCase(ServerParkName, QuestionnaireName, keyNames, keyValues, fieldData);

            // assert
            _blaiseCaseApiMock.Verify(v => v.CreateCase(primaryKeyValues, fieldData, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                string.Empty,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                null,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                string.Empty,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                null,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string>();
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'fieldData' must be supplied"));
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("fieldData"));
        }

        [Test]
        public void Given_Empty_keyNames_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyNames' must be supplied"));
        }

        [Test]
        public void Given_Null_keyNames_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                null,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("keyNames"));
        }

        [Test]
        public void Given_Empty_keyValues_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyValues' must be supplied"));
        }

        [Test]
        public void Given_Null_keyValues_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                null,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("keyValues"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var caseDtos = new List<CaseDto>();
            var caseDto = new CaseDto { CaseId = "1" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9998");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseDtos.Add(caseDto);

            caseDto = new CaseDto { CaseId = "2" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9999");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseDtos.Add(caseDto);

            // setup the mock behavior for RemoveCases and CreateCases methods
            _blaiseCaseApiMock.Setup(x => x.RemoveCases(QuestionnaireName, ServerParkName));
            _blaiseCaseApiMock.Setup(x => x.CreateCases(It.IsAny<List<CaseModel>>(), QuestionnaireName, ServerParkName));

            // act
            var result = _sut.CreateCases(caseDtos, QuestionnaireName, ServerParkName);

            // assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(QuestionnaireName, ServerParkName), Times.Once);

            const int BatchSize = 500;
            var expectedCreateCalls = (int)Math.Ceiling((double)caseDtos.Count / BatchSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), QuestionnaireName, ServerParkName), Times.Exactly(expectedCreateCalls));
            Assert.That(result, Is.EqualTo(caseDtos.Count));
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCases_Then_A_Bad_Request_Is_Returned()
        {
            // arrange
            var fieldData = new List<CaseDto>();

            // act
            var result = _sut.CreateCases(fieldData, QuestionnaireName, ServerParkName);

            // assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(QuestionnaireName, ServerParkName), Times.Once);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), QuestionnaireName, ServerParkName), Times.Never);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Given_A_Small_Batch_Size_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var caseDtos = new List<CaseDto>();
            var caseDto = new CaseDto { CaseId = "1" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9998");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseDtos.Add(caseDto);

            caseDto = new CaseDto { CaseId = "2" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9999");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseDtos.Add(caseDto);

            // set small batch size for testing
            const int BatchSize = 2;

            // act
            var result = _sut.CreateCases(caseDtos, QuestionnaireName, ServerParkName);

            // assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(QuestionnaireName, ServerParkName), Times.Once);

            var expectedCreateCalls = (int)Math.Ceiling((double)caseDtos.Count / BatchSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), QuestionnaireName, ServerParkName), Times.Exactly(expectedCreateCalls));

            Assert.That(result, Is.EqualTo(caseDtos.Count));
        }

        [Test]
        public void Given_A_Large_Batch_Size_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var caseDtoList = new List<CaseDto>();

            for (var iCounter = 1; iCounter <= 10000; iCounter++)
            {
                var caseDto = GenerateRandomCaseDto(iCounter);
                caseDtoList.Add(caseDto);
            }

            // set large batch size for testing
            const int BatchSize = 500;

            // act
            var result = _sut.CreateCases(caseDtoList, QuestionnaireName, ServerParkName);

            // assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(QuestionnaireName, ServerParkName), Times.Once);

            var expectedCreateCalls = (int)Math.Ceiling((double)caseDtoList.Count / BatchSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), QuestionnaireName, ServerParkName), Times.Exactly(expectedCreateCalls));

            Assert.That(result, Is.EqualTo(caseDtoList.Count));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateCase_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act
            _sut.UpdateCase(ServerParkName, QuestionnaireName, CaseId, fieldData);

            // assert
            _blaiseCaseApiMock.Verify(v => v.UpdateCase(primaryKey, fieldData, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                string.Empty,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                null,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                string.Empty,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                null,
                CaseId,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                string.Empty,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseId' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                null,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("caseId"));
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";
            var fieldData = new Dictionary<string, string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                CaseId,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'fieldData' must be supplied"));
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                CaseId,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("fieldData"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateCase__With_A_Multikey_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };
            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            // act
            _sut.UpdateCase(ServerParkName, QuestionnaireName, keyNames, keyValues, fieldData);

            // assert
            _blaiseCaseApiMock.Verify(v => v.UpdateCase(primaryKeyValues, fieldData, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                string.Empty,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                null,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                string.Empty,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_UpdateCase__With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                null,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string>();
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'fieldData' must be supplied"));
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("fieldData"));
        }

        [Test]
        public void Given_Empty_keyNames_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>();
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyNames' must be supplied"));
        }

        [Test]
        public void Given_Null_keyNames_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                null,
                keyValues,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("keyNames"));
        }

        [Test]
        public void Given_Empty_keyValues_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };
            var keyValues = new List<string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues,
                fieldData));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyValues' must be supplied"));
        }

        [Test]
        public void Given_Null_keyValues_When_I_Call_UpdateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                null,
                fieldData));
            Assert.That(exception.ParamName, Is.EqualTo("keyValues"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_DeleteCase_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);

            // act
            _sut.DeleteCase(ServerParkName, QuestionnaireName, CaseId);

            // assert
            _blaiseCaseApiMock.Verify(v => v.RemoveCase(primaryKey, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                string.Empty,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                null,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                ServerParkName,
                string.Empty,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                ServerParkName,
                null,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseId' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("caseId"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_DeleteCase_With_A_Multikey_Then_The_Correct_Service_Is_Called()
        {
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };
            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            // act
            _sut.DeleteCase(ServerParkName, QuestionnaireName, keyNames, keyValues);

            // assert
            _blaiseCaseApiMock.Verify(v => v.RemoveCase(primaryKeyValues, QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                string.Empty,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                null,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                ServerParkName,
                string.Empty,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                ServerParkName,
                null,
                keyNames,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_KeyNames_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyNames' must be supplied"));
        }

        [Test]
        public void Given_A_Null_KeyNames_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                null,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("keyNames"));
        }

        [Test]
        public void Given_An_Empty_KeyValues_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyValues' must be supplied"));
        }

        [Test]
        public void Given_A_Null_KeyValues_When_I_Call_DeleteCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("keyValues"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CaseExists_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);

            // act
            _sut.CaseExists(ServerParkName, QuestionnaireName, CaseId);

            // assert
            _blaiseCaseApiMock.Verify(v => v.CaseExists(primaryKey, QuestionnaireName, ServerParkName), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_CaseExists_Then_The_Expected_Value_Is_Returned(bool exists)
        {
            // arrange
            const string CaseId = "1000001";
            var primaryKey = PrimaryKeyHelper.CreatePrimaryKeys(CaseId);
            _blaiseCaseApiMock.Setup(c => c.CaseExists(primaryKey, QuestionnaireName, ServerParkName))
                .Returns(exists);

            // act
            var result = _sut.CaseExists(ServerParkName, QuestionnaireName, CaseId);

            // assert
            Assert.That(exists, Is.EqualTo(result));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                string.Empty,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(
                null,
                QuestionnaireName,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                ServerParkName,
                string.Empty,
                CaseId));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string CaseId = "1000001";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(
                ServerParkName,
                null,
                CaseId));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                ServerParkName,
                QuestionnaireName,
                string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseId' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(
                ServerParkName,
                QuestionnaireName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("caseId"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_CaseExists__With_a_Multikey_Then_The_Expected_Value_Is_Returned(bool exists)
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };
            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" },
            };

            _blaiseCaseApiMock.Setup(c => c.CaseExists(primaryKeyValues, QuestionnaireName, ServerParkName))
                .Returns(exists);

            // act
            var result = _sut.CaseExists(ServerParkName, QuestionnaireName, keyNames, keyValues);

            // assert
            Assert.That(exists, Is.EqualTo(result));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                string.Empty,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(
                null,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                ServerParkName,
                string.Empty,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(ServerParkName, null, keyNames, keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_keyNames_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyNames' must be supplied"));
        }

        [Test]
        public void Given_A_Null_keyNames_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(ServerParkName, QuestionnaireName, null, keyValues));
            Assert.That(exception.ParamName, Is.EqualTo("keyNames"));
        }

        [Test]
        public void Given_An_Empty_keyValues_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            var keyValues = new List<string>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(
                ServerParkName,
                QuestionnaireName,
                keyNames,
                keyValues));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'keyValues' must be supplied"));
        }

        [Test]
        public void Given_A_Null_keyValues_When_I_Call_CaseExists_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(ServerParkName, QuestionnaireName, keyNames, null));
            Assert.That(exception.ParamName, Is.EqualTo("keyValues"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditInformationList_Then_I_Get_A_List_Of_CaseEditingDetailsDto_Back()
        {
            // arrange
            var casesIds = new List<string>
            {
                "10001011",
            };

            var case1Mock = new Mock<IDataRecord>();

            _blaiseSqlApiMock.Setup(z => z.GetEditingCaseIds(QuestionnaireName)).Returns(casesIds);

            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(true);

            _dataSetMock.SetupSequence(d => d.ActiveRecord)
                .Returns(case1Mock.Object);

            _blaiseCaseApiMock.Setup(b => b.GetCases(QuestionnaireName, ServerParkName)).Returns(_dataSetMock.Object);
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case1Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } });

            // act
            var result = _sut.GetCaseEditInformationList(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<CaseEditInformationDto>>());
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditInformationList_Then_The_Correct_Services_Are_Called()
        {
            // arrange
            var casesIds = new List<string>
            {
                "10001011",
                "10001013",
            };

            var case1Mock = new Mock<IDataRecord>();
            var case2Mock = new Mock<IDataRecord>();
            var case3Mock = new Mock<IDataRecord>();

            _blaiseSqlApiMock.Setup(z => z.GetEditingCaseIds(QuestionnaireName)).Returns(casesIds);

            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(false)
                .Returns(false)
                .Returns(true);

            _dataSetMock.SetupSequence(d => d.ActiveRecord)
                            .Returns(case1Mock.Object)
                            .Returns(case2Mock.Object)
                            .Returns(case3Mock.Object);

            _blaiseCaseApiMock.Setup(b => b.GetCases(QuestionnaireName, ServerParkName)).Returns(_dataSetMock.Object);
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case1Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } });
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case2Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001012" } });
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case3Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001013" } });

            // act
            _sut.GetCaseEditInformationList(ServerParkName, QuestionnaireName);

            // assert
            _blaiseSqlApiMock.Verify(b => b.GetEditingCaseIds(QuestionnaireName), Times.Once);
            _blaiseCaseApiMock.Verify(b => b.GetCases(QuestionnaireName, ServerParkName), Times.Once);
            _mapperMock.Verify(b => b.MapToCaseEditInformationDto(case1Mock.Object), Times.Once);
            _mapperMock.Verify(b => b.MapToCaseEditInformationDto(case2Mock.Object), Times.Never);
            _mapperMock.Verify(b => b.MapToCaseEditInformationDto(case3Mock.Object), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCaseEditInformationList_Then_An_Expected_List_Of_EditingDetailsDto_Are_Returned()
        {
            // arrange
            var casesIds = new List<string>
            {
                "10001011",
                "10001013",
            };

            var case1Mock = new Mock<IDataRecord>();
            var case2Mock = new Mock<IDataRecord>();
            var case3Mock = new Mock<IDataRecord>();

            _blaiseSqlApiMock.Setup(z => z.GetEditingCaseIds(QuestionnaireName)).Returns(casesIds);

            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(false)
                .Returns(false)
                .Returns(true);

            _dataSetMock.SetupSequence(d => d.ActiveRecord)
                .Returns(case1Mock.Object)
                .Returns(case2Mock.Object)
                .Returns(case3Mock.Object);

            _blaiseCaseApiMock.Setup(b => b.GetCases(QuestionnaireName, ServerParkName)).Returns(_dataSetMock.Object);
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case1Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001011" } });
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case2Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001012" } });
            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValues(case3Mock.Object)).Returns(new Dictionary<string, string> { { "QID.Serial_Number", "10001013" } });

            var editingDetailsDto1 = new CaseEditInformationDto();
            var editingDetailsDto3 = new CaseEditInformationDto();

            _mapperMock.Setup(z => z.MapToCaseEditInformationDto(case1Mock.Object)).Returns(editingDetailsDto1);
            _mapperMock.Setup(z => z.MapToCaseEditInformationDto(case3Mock.Object)).Returns(editingDetailsDto3);

            // act
            var result = _sut.GetCaseEditInformationList(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(new List<CaseEditInformationDto> { editingDetailsDto1, editingDetailsDto3 }));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCaseEditInformationList_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            // act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCaseEditInformationList(ServerParkName, null));

            // assert
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCaseEditInformationList_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            // act
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCaseEditInformationList(null, QuestionnaireName));

            // assert
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCaseEditInformationList_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            // act
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCaseEditInformationList(ServerParkName, string.Empty));

            // assert
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCaseEditInformationList_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            // act
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCaseEditInformationList(string.Empty, QuestionnaireName));

            // assert
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        private static CaseDto GenerateRandomCaseDto(int caseId)
        {
            var caseDto = new CaseDto
            {
                CaseId = caseId.ToString(),
                FieldData = new Dictionary<string, string>
                {
                    { "qiD.Serial_Number", caseId.ToString() },
                    { "qDataBag.TLA", "LMS" },
                    { "qDataBag.PostCode", "TO41 7GH" },
                },
            };

            return caseDto;
        }
    }
}
