using Blaise.Api.Contracts.Interfaces;
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
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<IOnlineCaseService> _onlineCaseServiceMock;
        private Mock<ILoggingService> _loggingMock;

        private Mock<IDataRecord> _newDataRecordMock;
        private Mock<IDataRecord> _existingDataRecordMock;
        private Mock<IDataSet> _dataSetMock;

        private readonly string _primaryKey;
        private readonly string _databaseFileName;
        private readonly string _serverParkName;
        private readonly string _instrumentName;

        private CaseService _sut;

        public CaseServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
            _databaseFileName = "OPN123.bdbx";
        }

        [SetUp]
        public void SetUpTests()
        {
            _newDataRecordMock = new Mock<IDataRecord>();
            _existingDataRecordMock = new Mock<IDataRecord>();

            _dataSetMock = new Mock<IDataSet>();
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);

            _blaiseApiMock = new Mock<IBlaiseCaseApi>();
            _blaiseApiMock.Setup(b => b.GetCases(_databaseFileName)).Returns(_dataSetMock.Object);

            _onlineCaseServiceMock = new Mock<IOnlineCaseService>();
            _loggingMock = new Mock<ILoggingService>();

            _sut = new CaseService(
                _blaiseApiMock.Object,
                _onlineCaseServiceMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public void Given_There_Are_No_Records_Available_In_The_Nisra_File_When_I_Call_ImportOnlineDatabaseFile_Then_Nothing_Is_Processed()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(true);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _dataSetMock.Verify(v => v.EndOfSet, Times.Once);

            _blaiseApiMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_There_A_Record_In_The_Nisra_File_Does_Not_Exist_When_I_Call_ImportOnlineDatabaseFile_Then_The_Record_Is_Added()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValue(_newDataRecordMock.Object)).Returns(_primaryKey);
            _blaiseApiMock.Setup(b => b.CaseExists(_primaryKey, _serverParkName, _instrumentName)).Returns(false);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _blaiseApiMock.Verify(v => v.GetPrimaryKeyValue(_newDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.CaseExists(_primaryKey, _instrumentName, _serverParkName), Times.Once);

            _onlineCaseServiceMock.Verify(v => v.CreateOnlineCase(_newDataRecordMock.Object, _instrumentName,
                _serverParkName, _primaryKey), Times.Once);

            _dataSetMock.Verify(v => v.EndOfSet, Times.Exactly(2));
            _dataSetMock.Verify(v => v.ActiveRecord, Times.Once);
            _dataSetMock.Verify(v => v.MoveNext(), Times.Once);
            
            _blaiseApiMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_A_Record_Already_Exists_When_I_Call_ImportCasesFromFile_Then_The_Record_Is_Updated()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValue(_newDataRecordMock.Object)).Returns(_primaryKey);
            _blaiseApiMock.Setup(b => b.CaseExists(_primaryKey, _instrumentName, _serverParkName)).Returns(true);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _blaiseApiMock.Verify(v => v.GetPrimaryKeyValue(_newDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.CaseExists(_primaryKey, _instrumentName, _serverParkName), Times.Once);
            _blaiseApiMock.Verify(v => v.GetCase(_primaryKey, _instrumentName, _serverParkName), Times.Once);

            _dataSetMock.Verify(v => v.EndOfSet, Times.Exactly(2));
            _dataSetMock.Verify(v => v.ActiveRecord, Times.Once);
            _dataSetMock.Verify(v => v.MoveNext(), Times.Once);

            _onlineCaseServiceMock.Verify(v => v.UpdateExistingCaseWithOnlineData(_newDataRecordMock.Object, _existingDataRecordMock.Object,
                _serverParkName, _instrumentName, _primaryKey), Times.Once);

            _blaiseApiMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }
    }
}
