using System;
using System.Collections.Generic;
using System.Globalization;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Api.Tests.Unit.Helpers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class NisraFileImportServiceTests
    {
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<INisraCaseComparisonService> _caseComparisonServiceMock;
        private Mock<INisraCaseUpdateService> _onlineCaseServiceMock;
        private Mock<ILoggingService> _loggingServiceMock;
        
        private Mock<IDataRecord> _newDataRecordMock;
        private Mock<IDataRecord> _existingDataRecordMock;
        private Mock<IDataSet> _dataSetMock;

        private readonly string _primaryKey;
        private readonly string _databaseFileName;
        private readonly string _serverParkName;
        private readonly string _questionnaireName;

        private readonly CaseStatusModel _nisraCaseStatusModel;
        private readonly CaseStatusModel _existingStatusModel;

        private readonly IEnumerable<CaseStatusModel> _existingCaseStatusList;

        private NisraFileImportService _sut;

        public NisraFileImportServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _questionnaireName = "OPN123";
            _databaseFileName = "OPN123.bdbx";

            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);

            _nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, 110, DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture));
            _existingStatusModel = new CaseStatusModel(primaryKeyValues, 210, DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture));

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
            _blaiseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName)).Returns(_existingCaseStatusList);
            _blaiseApiMock.Setup(b => b.GetCaseStatus(_newDataRecordMock.Object)).Returns(_nisraCaseStatusModel);

            _caseComparisonServiceMock = new Mock<INisraCaseComparisonService>();

            _onlineCaseServiceMock = new Mock<INisraCaseUpdateService>();

            _loggingServiceMock = new Mock<ILoggingService>();

            _sut = new NisraFileImportService(
                _blaiseApiMock.Object,
                _caseComparisonServiceMock.Object,
                _onlineCaseServiceMock.Object,
                _loggingServiceMock.Object);
        }

        [Test]
        public void Given_There_Are_No_Records_Available_In_The_Nisra_File_When_I_Call_ImportNisraDatabaseFile_Then_Nothing_Is_Processed()
        {
            //arrange
            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(true);

            //act
            _sut.ImportNisraDatabaseFile(_databaseFileName, _questionnaireName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.GetCases(_databaseFileName), Times.Once);
            _dataSetMock.Verify(v => v.EndOfSet, Times.Once);

            _onlineCaseServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Given_The_Nisra_Record_Has_Updated_Data_When_I_Call_ImportNisraDatabaseFile_Then_The_Record_Is_Updated()
        {
            //arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);

            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _caseComparisonServiceMock.Setup(c =>
                    c.CaseNeedsToBeUpdated(_nisraCaseStatusModel, _existingStatusModel, _questionnaireName))
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetCase(primaryKeyValues, _questionnaireName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportNisraDatabaseFile(_databaseFileName, _questionnaireName, _serverParkName);

            //assert
            _onlineCaseServiceMock.Verify(v => v.UpdateCase(_newDataRecordMock.Object, _existingDataRecordMock.Object,
                _questionnaireName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_The_Nisra_Record_Has_Updated_Data_When_I_Call_ImportNisraDatabaseFile_Then_The_Record_Is_Not_Updated()
        {
            //arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);

            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _caseComparisonServiceMock.Setup(c =>
                    c.CaseNeedsToBeUpdated(_nisraCaseStatusModel, _existingStatusModel, _questionnaireName))
                .Returns(false);

            _blaiseApiMock.Setup(b => b.GetCase(primaryKeyValues, _questionnaireName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportNisraDatabaseFile(_databaseFileName, _questionnaireName, _serverParkName);

            //assert
            _onlineCaseServiceMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<IDataRecord>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void
            Given_A_Case_Exists_In_Nisra_But_Not_In_The_Database_When_I_Call_ImportNisraDatabaseFile_Then_Update_Is_Not_Called_And_A_Warning_Is_Logged()
        {
            //arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);

            var existingCaseStatusList = new List<CaseStatusModel>
            {
                new CaseStatusModel(PrimaryKeyHelper.CreatePrimaryKeys("0"), 110, DateTime.Now.ToString(CultureInfo.InvariantCulture))
            };

            _blaiseApiMock.Setup(b => b.GetCaseStatusModelList(_questionnaireName, _serverParkName)).Returns(existingCaseStatusList);

            _dataSetMock.Setup(d => d.ActiveRecord).Returns(_newDataRecordMock.Object);
            _dataSetMock.SetupSequence(d => d.EndOfSet)
                .Returns(false)
                .Returns(true);

            _blaiseApiMock.Setup(b => b.GetCase(primaryKeyValues, _questionnaireName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            //act
            _sut.ImportNisraDatabaseFile(_databaseFileName, _questionnaireName, _serverParkName);

            //assert
            _onlineCaseServiceMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<IDataRecord>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _loggingServiceMock.Verify(l => l.LogWarn($"The nisra case '{_nisraCaseStatusModel.PrimaryKey}' does not exist in the database for the questionnaire '{_questionnaireName}'"), Times.Once());
            _caseComparisonServiceMock.Verify(cc => cc.CaseNeedsToBeUpdated(It.IsAny<CaseStatusModel>(), It.IsAny<CaseStatusModel>(),
                It.IsAny<string>()), Times.Never);
        }
    }
}
