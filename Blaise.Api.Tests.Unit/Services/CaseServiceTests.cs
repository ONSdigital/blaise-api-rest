using System.Collections.Generic;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;

        private Mock<IBlaiseSqlApi> _blaiseSqlApiMock;

        private string _instrumentName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseSqlApiMock = new Mock<IBlaiseSqlApi>();

            _serverParkName = "LocalDevelopment";
            _instrumentName = "OPN2101A";

            _sut = new CaseService(_blaiseSqlApiMock.Object);
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
            var result = _sut.GetCaseIds(_instrumentName);

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
            var result = _sut.GetCaseIds(_instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
