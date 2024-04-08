using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Tests.Unit.Helpers;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;

        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<IDataRecord> _dataRecordMock;

        private string _questionnaireName;
        private string _serverParkName;
        private object list;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _dataRecordMock = new Mock<IDataRecord>();

            _serverParkName = "LocalDevelopment";
            _questionnaireName = "OPN2101A";

            _sut = new CaseService(_blaiseCaseApiMock.Object);
        }

        [Test]
        public void Given_A_Questionnaire_Has_Two_Cases_When_I_Call_GetCaseIds_Then_I_Get_A_List_Containing_Two_CaseIds_Back()
        {
            //arrange
            var caseStatusModelList = new List<CaseStatusModel>
            {
                
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000007"), 110, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000008"), 210, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
            };


            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName))
                .Returns(caseStatusModelList);

            //act
            var result = _sut.GetCaseIds(_serverParkName, _questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsInstanceOf<IEnumerable<string>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.Contains("0000007", result);
            Assert.Contains("0000008", result);
        }

        [Test]
        public void Given_A_Questionnaire_Has_No_Cases_When_I_Call_GetCaseIds_Then_I_Get_An_Empty_List_Back()
        {
            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName))
                .Returns(new List<CaseStatusModel>());

            //act
            var result = _sut.GetCaseIds(_serverParkName, _questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.IsInstanceOf<IEnumerable<string>>(result);
        }

        [Test]
        public void Given_A_Questionnaire_Has_Two_Cases_When_I_Call_GetCaseStatusList_Then_I_Get_A_List_Containing_Two_CaseStatusDtos_Back()
        {
            //arrange
            var caseStatusModelList = new List<CaseStatusModel>
            {
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000007"), 110, DateTime.Today.ToString(CultureInfo.InvariantCulture)),
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0000008"), 210, DateTime.Today.ToString(CultureInfo.InvariantCulture))
            };


            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName))
                .Returns(caseStatusModelList);

            //act
            var result = _sut.GetCaseStatusList(_serverParkName, _questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.IsInstanceOf<IEnumerable<CaseStatusDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(r => r.PrimaryKey == "0000007" && r.Outcome == 110));
            Assert.IsTrue(result.Any(r => r.PrimaryKey == "0000008" && r.Outcome == 210));
        }

        [Test]
        public void Given_A_Questionnaire_Has_No_Cases_When_I_Call_GetCaseStatusList_Then_I_Get_An_Empty_List_Back()
        {
            //arrange
            _blaiseCaseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName))
                .Returns(new List<CaseStatusModel>());

            //act
            var result = _sut.GetCaseStatusList(_serverParkName, _questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
            Assert.IsInstanceOf<IEnumerable<CaseStatusDto>>(result);
        }

        [Test]
        public void Given_I_Have_A_Case_With_A_PostCode_Set_When_I_Call_GetPostCode_Then_I_Get_The_PostCode_Back()
        {
            //arrange
            const string postCode = "NP1 0AA";
            var dataValueMock = new Mock<IDataValue>();
            dataValueMock.Setup(dv => dv.ValueAsText).Returns(postCode);

            var caseId = "0000007";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            _blaiseCaseApiMock.Setup(b => b.GetCase(primaryKeys, _questionnaireName, _serverParkName)).Returns(_dataRecordMock.Object);
            _blaiseCaseApiMock.Setup(f => f.GetFieldValue(_dataRecordMock.Object, FieldNameType.PostCode)).Returns(dataValueMock.Object);

            //act
            var result = _sut.GetPostCode(_serverParkName, _questionnaireName, caseId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postCode, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKeys, _questionnaireName, _serverParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetPrimaryKeyValues(_dataRecordMock.Object)).Returns(primaryKeys);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            //act
            _sut.GetCase(_serverParkName, _questionnaireName, caseId);

            //assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKeys, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Correct_CaseDto_Is_Returned()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKeys, _questionnaireName, _serverParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetPrimaryKeyValues(_dataRecordMock.Object)).Returns(primaryKeys);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            //act
            var result = _sut.GetCase(_serverParkName, _questionnaireName, caseId);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CaseDto>(result);
            Assert.AreEqual(caseId, result.CaseId);
            Assert.AreEqual(fieldData, result.FieldData);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(string.Empty,
                _questionnaireName, caseId));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(null,
                _questionnaireName, caseId));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName,
                string.Empty, caseId));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName,
                null, caseId));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName,
                _questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'caseId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName,
                _serverParkName, null));
            Assert.AreEqual("caseId", exception.ParamName);
        }


        [Test]
        public void Given_Multikey_Arguments_When_I_Call_GetCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            var primaryKeys = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" }
            };

            //act
            _sut.GetCase(_serverParkName, _questionnaireName, keyNames, keyValues);

            //assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(primaryKeys, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_Multikey_Arguments_When_I_Call_GetCase_Then_A_Correct_Case_Multikey_Dto_Is_Returned()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" }
            };

            _blaiseCaseApiMock.Setup(c => c.GetCase(primaryKeyValues, _questionnaireName, _serverParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);


            //act
            var result = _sut.GetCase(_serverParkName, _questionnaireName, keyNames, keyValues);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CaseMultikeyDto>(result);
            Assert.AreEqual(primaryKeyValues, result.PrimaryKeyValues);
            Assert.AreEqual(fieldData, result.FieldData);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCase_With_a_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange 
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(string.Empty,
                _questionnaireName, keyNames, keyValues));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCase__With_a_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(null,
                _questionnaireName, keyNames, keyValues));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName,
                string.Empty, keyNames, keyValues));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName,
                null, keyNames, keyValues));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_keyNames_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName, _questionnaireName,
                keyNames, keyValues));
            Assert.AreEqual("A value for the argument 'keyNames' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_keyNames_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName, _questionnaireName,
                null, keyValues));
            Assert.AreEqual("keyNames", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_keyValues_When_I_Call_GetCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };
            var keyValues = new List<string>();

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName, _questionnaireName,
                keyNames, keyValues));
            Assert.AreEqual("A value for the argument 'keyValues' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_keyValues_When_I_Call_GetCase_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName, _questionnaireName,
                keyNames, null));
            Assert.AreEqual("keyValues", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act
            _sut.CreateCase(_serverParkName, _questionnaireName, caseId, fieldData);

            //assert
            _blaiseCaseApiMock.Verify(v => v.CreateCase(primaryKeys, fieldData, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(string.Empty,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(null,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                string.Empty, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                null, caseId, fieldData));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _questionnaireName, string.Empty, fieldData));
            Assert.AreEqual("A value for the argument 'caseId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                _serverParkName, null, fieldData));
            Assert.AreEqual("caseId", exception.ParamName);
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string>();

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'fieldData' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                _serverParkName, caseId, null));
            Assert.AreEqual("fieldData", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCase_With_A_Multikey_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            var primaryKeyValues = new Dictionary<string, string>
            {
                { "mainSurveyId", "123-234343-343243" },
                { "id", "9000001" }
            };

            //act
            _sut.CreateCase(_serverParkName, _questionnaireName, keyNames, keyValues, fieldData);

            //assert
            _blaiseCaseApiMock.Verify(v => v.CreateCase(primaryKeyValues, fieldData, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(string.Empty,
                _questionnaireName, keyNames, keyValues, fieldData));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(null,
                _questionnaireName, keyNames, keyValues, fieldData));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                string.Empty, keyNames, keyValues, fieldData));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                null, keyNames, keyValues, fieldData));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string>();
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _questionnaireName, keyNames, keyValues, fieldData));
            Assert.AreEqual("A value for the argument 'fieldData' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                _serverParkName, keyNames, keyValues, null));
            Assert.AreEqual("fieldData", exception.ParamName);
        }

        [Test]
        public void Given_Empty_keyNames_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>();

            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _questionnaireName, keyNames, keyValues, fieldData));
            Assert.AreEqual("A value for the argument 'keyNames' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Null_keyNames_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyValues = new List<string>
            {
                "123-234343-343243",
                "9000001"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                _serverParkName, null, keyValues, fieldData));
            Assert.AreEqual("keyNames", exception.ParamName);
        }

        [Test]
        public void Given_Empty_keyValues_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            var keyValues = new List<string>();

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _questionnaireName, keyNames, keyValues, fieldData));
            Assert.AreEqual("A value for the argument 'keyValues' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Null_keyValues_When_I_Call_CreateCase_With_A_Multikey_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };
            var keyNames = new List<string>
            {
                "mainSurveyId",
                "id"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                _serverParkName, keyNames, null, fieldData));
            Assert.AreEqual("keyValues", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // Arrange
            var caseModelList = new List<CaseDto>();
            var caseDto = new CaseDto { CaseId = "1" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9998");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseModelList.Add(caseDto);

            caseDto = new CaseDto { CaseId = "2" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9999");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseModelList.Add(caseDto);


            // Set up the mock behavior for RemoveCases and CreateCases methods
            _blaiseCaseApiMock.Setup(x => x.RemoveCases(_questionnaireName, _serverParkName));
            _blaiseCaseApiMock.Setup(x => x.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName));

            // Act
            var result = _sut.CreateCases(caseModelList, this._questionnaireName, this._serverParkName);

            // Assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(_questionnaireName, _serverParkName), Times.Once);


            var batchSize = 500;
            var expectedCreateCalls = (int)Math.Ceiling((double)caseModelList.Count / batchSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName), Times.Exactly(expectedCreateCalls));
            Assert.AreEqual(caseModelList.Count, result);
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_CreateCases_The_A_Bad_Request_Is_Returned()
        {
            // Arrange
            var fieldData = new List<CaseDto>();

            // Act
            var result = _sut.CreateCases(fieldData, this._questionnaireName, this._serverParkName);

            // Assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(_questionnaireName, _serverParkName), Times.Once);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName), Times.Never);
            Assert.AreEqual(0, result);
        }


        [Test]
        public void Given_A_Small_Batch_Size_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // Arrange
            var caseModelList = new List<CaseDto>();
            var caseDto = new CaseDto { CaseId = "1" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9998");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseModelList.Add(caseDto);

            caseDto = new CaseDto { CaseId = "2" };
            caseDto.FieldData.Add("qiD.Serial_Number", "9999");
            caseDto.FieldData.Add("qDataBag.TLA", "LMS");
            caseDto.FieldData.Add("qDataBag.PostCode", "TO41 7GH");
            caseModelList.Add(caseDto);

            // Set a small batch size for testing
            var maxChunkSize = 2;

            // Act
            var result = _sut.CreateCases(caseModelList, this._questionnaireName, this._serverParkName);

            // Assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(_questionnaireName, _serverParkName), Times.Once);

            var expectedCreateCalls = (int)Math.Ceiling((double)caseModelList.Count / maxChunkSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName), Times.Exactly(expectedCreateCalls));

            Assert.AreEqual(caseModelList.Count, result);
        }

        [Test]
        public void Given_A_Large_Batch_Size_When_I_Call_CreateCases_Then_The_Correct_Service_Is_Called()
        {
            // Arrange
            var caseDtoList = new List<CaseDto>();

            for (var iCounter = 1; iCounter <= 10000; iCounter++)
            {
                var caseDto = GenerateRandomCaseDto(iCounter);
                caseDtoList.Add(caseDto);
            }

            // Set a large batch size for testing
            var maxChunkSize = 500;

            // Act
            var result = _sut.CreateCases(caseDtoList, this._questionnaireName, this._serverParkName);

            // Assert
            _blaiseCaseApiMock.Verify(x => x.RemoveCases(_questionnaireName, _serverParkName), Times.Once);

            var expectedCreateCalls = (int)Math.Ceiling((double)caseDtoList.Count / maxChunkSize);
            _blaiseCaseApiMock.Verify(x => x.CreateCases(It.IsAny<List<CaseModel>>(), _questionnaireName, _serverParkName), Times.Exactly(expectedCreateCalls));

            Assert.AreEqual(caseDtoList.Count, result);
        }

        static CaseDto GenerateRandomCaseDto(int caseId)
        {
            var caseDto = new CaseDto
            {
                CaseId = caseId.ToString(),
                FieldData = new Dictionary<string, string>
                                {
                                    { "qiD.Serial_Number", caseId.ToString() },
                                    {"qDataBag.TLA", "LMS"},
                                    {"qDataBag.PostCode", "TO41 7GH"}
                                }
            };

            return caseDto;
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act
            _sut.UpdateCase(_serverParkName, _questionnaireName, caseId, fieldData);

            //assert
            _blaiseCaseApiMock.Verify(v => v.UpdateCase(primaryKeys, fieldData, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(string.Empty,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(null,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(_serverParkName,
                string.Empty, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(_serverParkName,
                null, caseId, fieldData));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(_serverParkName,
                _questionnaireName, string.Empty, fieldData));
            Assert.AreEqual("A value for the argument 'caseId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(_serverParkName,
                _serverParkName, null, fieldData));
            Assert.AreEqual("caseId", exception.ParamName);
        }

        [Test]
        public void Given_Empty_FieldData_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string>();

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(_serverParkName,
                _questionnaireName, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'fieldData' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Null_FieldData_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(_serverParkName,
                _serverParkName, caseId, null));
            Assert.AreEqual("fieldData", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_DeleteCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);

            //act
            _sut.DeleteCase(_serverParkName, _questionnaireName, caseId);

            //assert
            _blaiseCaseApiMock.Verify(v => v.RemoveCase(primaryKeys, _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(string.Empty,
                _questionnaireName, caseId));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(null,
                _questionnaireName, caseId));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(_serverParkName,
                string.Empty, caseId));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(_serverParkName,
                null, caseId));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_DeleteCase_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeleteCase(_serverParkName,
                _questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'caseId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_DeleteCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeleteCase(_serverParkName,
                _serverParkName, null));
            Assert.AreEqual("caseId", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_CaseExists_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);

            //act
            _sut.CaseExists(_serverParkName, _questionnaireName, caseId);

            //assert
            _blaiseCaseApiMock.Verify(v => v.CaseExists(primaryKeys, _questionnaireName, _serverParkName), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_CaseExists_Then_The_Expected_Value_Is_Returned(bool exists)
        {
            //arrange
            const string caseId = "1000001";
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseId);
            _blaiseCaseApiMock.Setup(c => c.CaseExists(primaryKeys, _questionnaireName, _serverParkName))
                .Returns(exists);

            //act
            var result = _sut.CaseExists(_serverParkName, _questionnaireName, caseId);

            //assert
            Assert.AreEqual(result, exists);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(string.Empty,
                _questionnaireName, caseId));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(null,
                _questionnaireName, caseId));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(_serverParkName,
                string.Empty, caseId));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(_serverParkName,
                null, caseId));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_CaseExists_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CaseExists(_serverParkName,
                _questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'caseId' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_CaseExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CaseExists(_serverParkName,
                _serverParkName, null));
            Assert.AreEqual("caseId", exception.ParamName);
        }
    }
}
