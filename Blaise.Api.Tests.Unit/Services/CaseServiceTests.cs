using System;
using System.Collections.Generic;
using System.Globalization;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<ICaseComparisonService> _caseComparisonServiceMock;
        private Mock<IOnlineCaseUpdateService> _onlineCaseServiceMock;
        
        private Mock<IDataRecord> _newDataRecordMock;
        private Mock<IDataRecord> _existingDataRecordMock;
        private Mock<IDataSet> _dataSetMock;

        private readonly string _primaryKey;
        private readonly string _databaseFileName;
        private readonly string _serverParkName;
        private readonly string _instrumentName;

        private readonly CaseStatusModel _nisraCaseStatusModel;
        private readonly CaseStatusModel _existingStatusModel;

        private readonly IEnumerable<CaseStatusModel> _existingCaseStatusList;

        private CaseService _sut;

        public CaseServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
            _databaseFileName = "OPN123.bdbx";

            _nisraCaseStatusModel = new CaseStatusModel(_primaryKey, 110, DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture));
            _existingStatusModel = new CaseStatusModel(_primaryKey, 210, DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture));

            _existingCaseStatusList = new List<CaseStatusModel>
             {
                 _existingStatusModel
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
            _blaiseApiMock.Setup(b => b.GetCaseStatusList(_instrumentName, _serverParkName)).Returns(_existingCaseStatusList);
            _blaiseApiMock.Setup(b => b.GetCaseStatus(_newDataRecordMock.Object)).Returns(_nisraCaseStatusModel);

            _caseComparisonServiceMock = new Mock<ICaseComparisonService>();

            _onlineCaseServiceMock = new Mock<IOnlineCaseUpdateService>();

            _sut = new CaseService(
                _blaiseApiMock.Object,
                _caseComparisonServiceMock.Object,
                _onlineCaseServiceMock.Object);
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

            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_The_Nisra_Record_Has_Updated_Data_When_I_Call_ImportOnlineDatabaseFile_Then_The_Record_Is_Updated()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _caseComparisonServiceMock.Setup(c =>
                    c.UpdateExistingCase(_nisraCaseStatusModel, _existingStatusModel, _instrumentName))
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _onlineCaseServiceMock.Verify(v => v.UpdateCase(_newDataRecordMock.Object, _existingDataRecordMock.Object,
                _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_The_Nisra_Record_Has_Updated_Data_When_I_Call_ImportCasesFromFile_Then_The_Record_Is_Not_Updated()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _caseComparisonServiceMock.Setup(c =>
                    c.UpdateExistingCase(_nisraCaseStatusModel, _existingStatusModel, _instrumentName))
                .Returns(false);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportOnlineDatabaseFile(_databaseFileName, _instrumentName, _serverParkName);

            //assert
            _onlineCaseServiceMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<IDataRecord>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
