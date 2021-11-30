using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;
using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;

        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<IDataSet> _dataSetMock;
        private Mock<IDataRecord> _dataRecordMock;

        private string _instrumentName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _dataSetMock = new Mock<IDataSet>();
            _dataRecordMock = new Mock<IDataRecord>();

            _serverParkName = "LocalDevelopment";
            _instrumentName = "OPN2101A";

            _sut = new CaseService(_blaiseCaseApiMock.Object);
        }

        [Test]
        public void Given_An_Instrument_Has_Two_Cases_When_I_Call_GetCaseIds_Then_I_Get_A_List_Containing_Two_CaseIds_Back()
        {
            //arrange
            var primaryKey1 = "0000007";
            var primaryKey2 = "0000077";
            var dataRecord1Mock = new Mock<IDataRecord>();
            var dataRecord2Mock = new Mock<IDataRecord>();

            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(false)
                .Returns(true);

            _dataSetMock.SetupSequence(b => b.ActiveRecord)
                .Returns(dataRecord1Mock.Object)
                .Returns(dataRecord2Mock.Object);

            _blaiseCaseApiMock.Setup(b => b.GetCases(_instrumentName, _serverParkName))
                .Returns(_dataSetMock.Object);

            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValue(dataRecord1Mock.Object))
                .Returns(primaryKey1);

            _blaiseCaseApiMock.Setup(b => b.GetPrimaryKeyValue(dataRecord2Mock.Object))
                .Returns(primaryKey2);

            //act
            var result = _sut.GetCaseIds(_serverParkName, _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
            Assert.Contains(primaryKey1, result);
            Assert.Contains(primaryKey2, result);
        }

        [Test]
        public void Given_An_Instrument_Has_No_Cases_When_I_Call_CaseIds_Then_I_Get_An_Empty_List_Back()
        {
            //arrange
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(true);

            _blaiseCaseApiMock.Setup(b => b.GetCases(_instrumentName, _serverParkName))
                .Returns(_dataSetMock.Object);

            //act
            var result = _sut.GetCaseIds(_serverParkName, _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Given_I_Have_A_Case_With_A_PostCode_Set_When_I_Call_GetPostCode_Then_I_Get_The_PostCode_Back()
        {
            //arrange
            const string postCode = "NP1 0AA";
            var dataValueMock = new Mock<IDataValue>();
            dataValueMock.Setup(dv => dv.ValueAsText).Returns(postCode);

            var caseId = "0000007";
            _blaiseCaseApiMock.Setup(b => b.GetCase(caseId, _instrumentName, _serverParkName)).Returns(_dataRecordMock.Object);
            _blaiseCaseApiMock.Setup(f => f.GetFieldValue(_dataRecordMock.Object, FieldNameType.PostCode)).Returns(dataValueMock.Object);

            //act
            var result = _sut.GetPostCode(_serverParkName, _instrumentName, caseId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postCode, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetCase(caseId, _instrumentName, _serverParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetPrimaryKeyValue(_dataRecordMock.Object)).Returns(caseId);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            //act
            _sut.GetCase(_serverParkName, _instrumentName, caseId);

            //assert
            _blaiseCaseApiMock.Verify(v => v.GetCase(caseId, _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetCase_Then_The_Correct_CaseDto_Is_Returned()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            _blaiseCaseApiMock.Setup(c => c.GetCase(caseId, _instrumentName, _serverParkName))
                .Returns(_dataRecordMock.Object);

            _blaiseCaseApiMock.Setup(c => c.GetPrimaryKeyValue(_dataRecordMock.Object)).Returns(caseId);

            _blaiseCaseApiMock.Setup(c => c.GetRecordDataFields(_dataRecordMock.Object)).Returns(fieldData);

            //act
            var result = _sut.GetCase(_serverParkName, _instrumentName, caseId);

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
                _instrumentName, caseId));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(null,
                _instrumentName, caseId));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName,
                string.Empty, caseId));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_GetCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCase(_serverParkName,
                null, caseId));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_GetCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCase(_serverParkName,
                _instrumentName, string.Empty));
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
        public void Given_Valid_Arguments_When_I_Call_CreateCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act
            _sut.CreateCase(_serverParkName, _instrumentName, caseId, fieldData);

            //assert
            _blaiseCaseApiMock.Verify(v => v.CreateCase(caseId, fieldData, _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(string.Empty,
                _instrumentName, caseId, fieldData));
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
                _instrumentName, caseId, fieldData));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                string.Empty, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_CreateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateCase(_serverParkName,
                null, caseId, fieldData));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_CreateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateCase(_serverParkName,
                _instrumentName, string.Empty, fieldData));
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
                _instrumentName, caseId, fieldData));
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
        public void Given_Valid_Arguments_When_I_Call_UpdateCase_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act
            _sut.UpdateCase(_serverParkName, _instrumentName, caseId, fieldData);

            //assert
            _blaiseCaseApiMock.Verify(v => v.UpdateCase(caseId, fieldData, _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(string.Empty,
                _instrumentName, caseId, fieldData));
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
                _instrumentName, caseId, fieldData));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_InstrumentName_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(_serverParkName,
                string.Empty, caseId, fieldData));
            Assert.AreEqual("A value for the argument 'instrumentName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_InstrumentName_When_I_Call_UpdateCase_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string caseId = "1000001";
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateCase(_serverParkName,
                null, caseId, fieldData));
            Assert.AreEqual("instrumentName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_CaseId_When_I_Call_UpdateCase_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var fieldData = new Dictionary<string, string> { { "yo", "man" } };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateCase(_serverParkName,
                _instrumentName, string.Empty, fieldData));
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
                _instrumentName, caseId, fieldData));
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
    }
}
