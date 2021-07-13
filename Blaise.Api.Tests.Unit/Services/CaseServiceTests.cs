using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
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
        private Mock<IDataSet> _dataSetMock;

        private string _instrumentName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            //Setup mocks
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _dataSetMock = new Mock<IDataSet>();

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
    }
}
