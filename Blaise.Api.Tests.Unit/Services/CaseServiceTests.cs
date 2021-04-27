using System;
using System.Collections.Generic;
using System.Globalization;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Enums;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Models;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Exceptions;
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

        private readonly int _nisraOutcome;
        private readonly int _existingOutcome;

        private readonly string _nisraLastUpdated;
        private readonly string _existingLastUpdated;
        
        private readonly IEnumerable<CaseComparisonModel> _existingCaseDetails;

        private CaseService _sut;

        public CaseServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
            _databaseFileName = "OPN123.bdbx";

            _nisraOutcome = 110;
            _existingOutcome = 210;

            _nisraLastUpdated = DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture);
            _existingLastUpdated = DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture);

            _existingCaseDetails = new List<CaseComparisonModel>
             {
                 new CaseComparisonModel(_primaryKey, _existingOutcome, ModeType.Tel, _existingLastUpdated)
             };
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
            _onlineCaseServiceMock.Setup(o => o.GetExistingCaseComparisonModels(_instrumentName, _serverParkName))
                .Returns(_existingCaseDetails);

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
        public void Given_A_Record_Already_Exists_When_I_Call_ImportCasesFromFile_Then_The_Record_Is_Updated()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValue(_newDataRecordMock.Object)).Returns(_primaryKey);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_newDataRecordMock.Object)).Returns(_nisraOutcome);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_newDataRecordMock.Object)).Returns(_nisraLastUpdated);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _blaiseApiMock.Verify(v => v.GetPrimaryKeyValue(_newDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetCase(_primaryKey, _instrumentName, _serverParkName), Times.Once);

            _dataSetMock.Verify(v => v.EndOfSet, Times.Exactly(2));
            _dataSetMock.Verify(v => v.ActiveRecord, Times.Once);
            _dataSetMock.Verify(v => v.MoveNext(), Times.Once);

            _onlineCaseServiceMock.Verify(v => v.UpdateCase(_newDataRecordMock.Object, _existingDataRecordMock.Object,
                _instrumentName, _serverParkName, _nisraOutcome, _existingOutcome, _primaryKey), Times.Once);

            _blaiseApiMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_A_Record_Does_Not_Exist_When_I_Call_ImportCasesFromFile_Then_The_Next_Record_Is_Processed()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValue(_newDataRecordMock.Object)).Returns(_primaryKey);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Throws(new DataNotFoundException(""));

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _blaiseApiMock.Verify(v => v.GetPrimaryKeyValue(_newDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetCase(_primaryKey, _instrumentName, _serverParkName), Times.Once);

            _dataSetMock.Verify(v => v.EndOfSet, Times.Exactly(2));
            _dataSetMock.Verify(v => v.ActiveRecord, Times.Once);
            _dataSetMock.Verify(v => v.MoveNext(), Times.Once);

            _onlineCaseServiceMock.Verify(v => v.UpdateCase(_newDataRecordMock.Object, _existingDataRecordMock.Object,
                _instrumentName, _serverParkName, _nisraOutcome, _existingOutcome, _primaryKey), Times.Never);

            _blaiseApiMock.VerifyNoOtherCalls();
            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        // Scenario 11
        //[Test]
        //public void Given_Nisra_Is_Superior_But_The_Case_Is_In_Use_In_Cati_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_Case_Is_Not_Updated()
        //{
        //    //arrange
        //    const int hOutComplete = 110; //complete
        //    const int hOutPartial = 210; //partial


        //    //set up case is in use
        //    _blaiseApiMock.Setup(b => b.CaseInUseInCati(_existingDataRecordMock.Object))
        //        .Returns(true);

        //    //act
        //    _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _serverParkName,
        //        _instrumentName, _primaryKey);

        //    //assert
        //    _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
        //        _instrumentName, _serverParkName), Times.Never);
        //}
    }
}
