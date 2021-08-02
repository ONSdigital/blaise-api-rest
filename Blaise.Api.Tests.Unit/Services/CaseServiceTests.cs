using System.Collections.Generic;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;

        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<IBlaiseSqlApi> _blaiseSqlApiMock;
        private Mock<IDataRecord> _dataRecordMock;

        private string _instrumentName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _blaiseSqlApiMock = new Mock<IBlaiseSqlApi>();
            _dataRecordMock = new Mock<IDataRecord>();

            _serverParkName = "LocalDevelopment";
            _instrumentName = "OPN2101A";

            _sut = new CaseService(_blaiseCaseApiMock.Object, _blaiseSqlApiMock.Object);
        }

        [Test]
        public void Given_An_Instrument_Has_Two_Cases_When_I_Call_GetCaseIds_Then_I_Get_A_List_Containing_Two_CaseIds_Back()
        {
            //arrange
            var caseIdList = new List<string>
            {
                "0000007",
                "0000008"
            };

            _blaiseSqlApiMock.Setup(sql => sql.GetCaseIds(_instrumentName)).Returns(caseIdList);
           
            //act
            var result = _sut.GetCaseIds(_serverParkName, _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
            Assert.Contains(caseIdList[0], result);
            Assert.Contains(caseIdList[1], result);
        }

        [Test]
        public void Given_An_Instrument_Has_No_Cases_When_I_Call_CaseIds_Then_I_Get_An_Empty_List_Back()
        {
            //arrange
            var caseIdList = new List<string>();

            _blaiseSqlApiMock.Setup(sql => sql.GetCaseIds(_instrumentName)).Returns(caseIdList);


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

            const string caseId = "0000007";
            _blaiseCaseApiMock.Setup(b => b.GetCase(caseId, _instrumentName, _serverParkName)).Returns(_dataRecordMock.Object);
            _blaiseCaseApiMock.Setup(f => f.GetFieldValue(_dataRecordMock.Object, FieldNameType.PostCode)).Returns(dataValueMock.Object);

            //act
            var result = _sut.GetPostCode(_serverParkName, _instrumentName, caseId);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postCode, result);
        }
    }
}
