using System;
using System.Collections.Generic;
using System.Globalization;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class NisraCaseUpdateServiceTests
    {
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<ICatiDataBlockService> _catiDataMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private Mock<IDataRecord> _nisraDataRecordMock;
        private Mock<IDataRecord> _existingDataRecordMock;
       
        private readonly string _primaryKey;
        private readonly string _serverParkName;
        private readonly string _instrumentName;
        private readonly int _outcomeCode;
        private readonly string _lastUpdated;

        private Dictionary<string, string> _newFieldData;
        private Dictionary<string, string> _existingFieldData;
        
        private NisraCaseUpdateService _sut;

        public NisraCaseUpdateServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
            _outcomeCode = 110;
            _lastUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseCaseApi>();
            _loggingMock = new Mock<ILoggingService>();

            //set up new record
            _nisraDataRecordMock = new Mock<IDataRecord>();
            _newFieldData = new Dictionary<string, string>();
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object)).Returns(_newFieldData);
            _blaiseApiMock.Setup(b => b.GetPrimaryKeyValue(It.IsAny<IDataRecord>())).Returns(_primaryKey);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(It.IsAny<IDataRecord>())).Returns(_outcomeCode);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(It.IsAny<IDataRecord>())).Returns(_lastUpdated);

            //set up existing record
            _existingDataRecordMock = new Mock<IDataRecord>();
            _existingFieldData = new Dictionary<string, string>();
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object))
                .Returns(_existingFieldData);

            //important that the service calls the methods in the right order, otherwise you could end up removing what you have added
            _catiDataMock = new Mock<ICatiDataBlockService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveCallHistoryBlock(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveWebNudgedField(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                It.IsAny<int>()));

            _sut = new NisraCaseUpdateService(
                _blaiseApiMock.Object,
                _catiDataMock.Object,
                _loggingMock.Object);
        }

        // Scenario 11
        [Test]
        public void Given_The_Case_Is_Open_In_Cati_When_I_Call_UpdateCase_Then_The_Case_Is_Not_Updated()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.CaseInUseInCati(_existingDataRecordMock.Object)).Returns(true);

            //act
            _sut.UpdateCase(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _instrumentName, _serverParkName);

            //assert
            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Never);
        }

        [Test]
        public void Given_The_Case_Is_Not_Open_In_Cati_When_I_Call_UpdateCase_Then_The_Case_Is_Updated()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.CaseInUseInCati(_existingDataRecordMock.Object)).Returns(false);
           
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object)).Returns(_newFieldData);
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object)).Returns(_existingFieldData);
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                _outcomeCode));
            _blaiseApiMock.Setup(b => b.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName));

            //act
            _sut.UpdateCase(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _instrumentName, _serverParkName);

            //assert
            _catiDataMock.Verify(v => v.RemoveCatiManaBlock(_newFieldData), Times.Once);
            _catiDataMock.Verify(v => v.RemoveCallHistoryBlock(_newFieldData), Times.Once);
            _catiDataMock.Verify(v => v.RemoveWebNudgedField(_newFieldData), Times.Once);
            _catiDataMock.Verify(v => v.AddCatiManaCallItems(_newFieldData, _existingFieldData, _outcomeCode), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        [Test]
        public void Given_The_Case_Is_Not_Open_In_Cati_And_The_Record_Gets_Updated_When_I_Call_UpdateCase_Then_The_Update_Is_Logged()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.CaseInUseInCati(_existingDataRecordMock.Object)).Returns(false);

            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            var lastUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_nisraDataRecordMock.Object)).Returns(lastUpdated);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_existingDataRecordMock.Object)).Returns(lastUpdated);

            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object)).Returns(_newFieldData);
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object)).Returns(_existingFieldData);
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                _outcomeCode));
            _blaiseApiMock.Setup(b => b.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName));

            //act
            _sut.UpdateCase(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _instrumentName, _serverParkName);

            //assert
            _loggingMock.Verify(v => v.LogInfo($"NISRA case '{_primaryKey}' was successfully updated for instrument '{_instrumentName}'"),
                Times.Once);

            _loggingMock.Verify(v => v.LogWarn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Given_The_Case_Is_Not_Open_In_Cati_And_The_Record_Does_Not_Get_Updated_When_I_Call_UpdateCase_Then_A_Warning_Is_Logged()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.CaseInUseInCati(_existingDataRecordMock.Object)).Returns(false);

            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object)).Returns(_newFieldData);
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object)).Returns(_existingFieldData);
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiDataMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                _outcomeCode));

            //set the return of the date field to be a different value
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_existingDataRecordMock.Object)).Returns(DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture));
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_nisraDataRecordMock.Object)).Returns(DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture));

            //act
            _sut.UpdateCase(_nisraDataRecordMock.Object, _existingDataRecordMock.Object,_instrumentName, _serverParkName);

            //assert
            _loggingMock.Verify(v => v.LogWarn($"NISRA case '{_primaryKey}' failed to update - potentially open in Cati at the time of the update for instrument '{_instrumentName}'"),
                Times.Once);
        }

        [Test]
        public void Given_A_Record_Has_Updated_When_I_Call_RecordHasBeenUpdated_Then_True_Is_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            var lastUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_nisraDataRecordMock.Object)).Returns(lastUpdated);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_existingDataRecordMock.Object)).Returns(lastUpdated);

            //act
            var result = _sut.RecordHasBeenUpdated(_primaryKey, _nisraDataRecordMock.Object, _instrumentName, _serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.True(result);
        }

        [Test]
        public void Given_A_Record_Has_Not_Updated_Due_To_Different_LastUpdated_dates_When_I_Call_RecordHasBeenUpdated_Then_False_Is_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetCase(_primaryKey, _instrumentName, _serverParkName))
                .Returns(_existingDataRecordMock.Object);

            var nisraLastUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_nisraDataRecordMock.Object)).Returns(nisraLastUpdated);

            var existingLastUpdated = DateTime.Now.AddMinutes(-30).ToString(CultureInfo.InvariantCulture);
            _blaiseApiMock.Setup(b => b.GetLastUpdatedAsString(_existingDataRecordMock.Object)).Returns(existingLastUpdated);

            //act
            var result = _sut.RecordHasBeenUpdated(_primaryKey, _nisraDataRecordMock.Object,  _instrumentName, _serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.False(result);
        }
    }
}
