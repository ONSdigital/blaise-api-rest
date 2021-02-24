using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class UpdateCaseServiceTests
    {
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<ICatiDataService> _catiManaMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private Mock<IDataRecord> _nisraDataRecordMock;
        private Mock<IDataRecord> _existingDataRecordMock;
        private Mock<IDataValue> _dataValueMock;

        private readonly string _primaryKey;
        private readonly string _serverParkName;
        private readonly string _instrumentName;
        private Dictionary<string, string> _newFieldData;
        private Dictionary<string, string> _existingFieldData;

        private UpdateCaseService _sut;

        public UpdateCaseServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
        }

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseCaseApi>();
            _loggingMock = new Mock<ILoggingService>();
     
            //set up new record
            _nisraDataRecordMock = new Mock<IDataRecord>();
            _newFieldData = new Dictionary<string, string>();
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object))
                .Returns(_newFieldData);

            //set up existing record
            _existingDataRecordMock = new Mock<IDataRecord>();
            _existingFieldData = new Dictionary<string, string>();
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object))
                .Returns(_existingFieldData);

            //set up case not in use
            _dataValueMock = new Mock<IDataValue>();
            _dataValueMock.Setup(d => d.IntegerValue).Returns(0); 
            _blaiseApiMock.Setup(b => b.GetFieldValue(_existingDataRecordMock.Object, FieldNameType.CaseInUse))
                .Returns(_dataValueMock.Object);

            //important that the service calls the methods in the right order, otherwise you could end up removing what you have added
            _catiManaMock = new Mock<ICatiDataService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _catiManaMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.RemoveCallHistoryBlock(_newFieldData));
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.RemoveWebNudgedField(_newFieldData));
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                It.IsAny<int>()));
            
            _sut = new UpdateCaseService(
                _blaiseApiMock.Object,
                _catiManaMock.Object,
                _loggingMock.Object);
        }

        // Scenario 1 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Complete_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated()
        {
            //arrange
            const int hOutComplete = 110; //complete

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutComplete);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutComplete);
            
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _serverParkName, 
                _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 2 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Complete_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Not_Updated()
        {
            //arrange
            const int hOutPartial = 210; //partial
            const int hOutComplete = 110; //complete

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutPartial);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutComplete);

            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<Dictionary<string, string>>(), 
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // Scenario 3 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Partial_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated()
        {
            //arrange
            const int hOutPartial = 210; //partial
            const int hOutComplete = 110; //complete

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutComplete);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutPartial);
            
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, 
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 4  (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Between_210_And_542_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated(int existingOutcome)
        {
            //arrange
            const int hOutComplete = 110; //complete

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutComplete);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(existingOutcome);
          
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object,
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 5 & 8 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(110)]
        [TestCase(310)]
        public void Given_The_Nisra_Outcome_Is_Zero_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_Existing_Record_Is_Not_Updated(
            int existingOutcome)
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(0);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(existingOutcome);

            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, 
                _serverParkName, _instrumentName,
                _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
           
            _blaiseApiMock.VerifyNoOtherCalls();

        }

        // Scenario 6 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Partial_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated()
        {
            //arrange
            const int hOutPartial = 210; //partial

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutPartial);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutPartial);
           
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object,
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 7 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Haw_An_Outcome_Between_210_And_542_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated(int existingOutcome)
        {
            //arrange
            const int hOutPartial = 210; //partial

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutPartial);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(existingOutcome);
            
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object,
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 8 - covered by Scenario 5 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)

        //additional scenario
        [TestCase(110)]
        [TestCase(210)]
        public void Given_The_Nisra_Case_Has_A_Valid_Outcome_But_Existing_Case_Haw_An_Outcome_Of_Zero_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Updated(int nisraOutcome)
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(nisraOutcome);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(0);
            
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, 
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Once);
        }

        // Scenario 9 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Not_Updated()
        {
            //arrange
            const int hOutPartial = 210; //partial
            const int hOutComplete = 562; //respondent request for data to be deleted

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutPartial);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutComplete);

            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object,
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<Dictionary<string, string>>(), 
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // Scenario 10 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_To_Record_Is_Not_Updated()
        {
            //arrange
            const int hOutPartial = 110; //Complete
            const int hOutComplete = 561; //respondent request for data to be deleted

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutPartial);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutComplete);

            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, 
                _serverParkName, _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.UpdateCase(It.IsAny<IDataRecord>(), It.IsAny<Dictionary<string, string>>(), 
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        // Scenario 11
        [Test]
        public void Given_Nisra_Is_Superior_But_The_Case_Is_In_Use_In_Cati_When_I_Call_UpdateExistingCaseWithOnlineData_Then_The_Case_Is_Not_Updated()
        {
            //arrange
            const int hOutComplete = 110; //complete

            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(hOutComplete);
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_existingDataRecordMock.Object)).Returns(hOutComplete);

            //set up case is in use
            _dataValueMock.Setup(d => d.IntegerValue).Returns(1); 
            _blaiseApiMock.Setup(b => b.GetFieldValue(_existingDataRecordMock.Object, FieldNameType.CaseInUse))
                .Returns(_dataValueMock.Object);
            
            //act
            _sut.UpdateExistingCaseWithOnlineData(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, _serverParkName, 
                _instrumentName, _primaryKey);

            //assert
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_nisraDataRecordMock.Object), Times.Once);
            _blaiseApiMock.Verify(v => v.GetOutcomeCode(_existingDataRecordMock.Object), Times.Never);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData,
                _instrumentName, _serverParkName), Times.Never);
        }

        [Test]
        public void Given_I_Call_UpdateCase_Then_The_Case_Is_Updated()
        {
            //arrange
            const int outcomeCode = 110;

            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_nisraDataRecordMock.Object)).Returns(_newFieldData);
            _blaiseApiMock.Setup(b => b.GetRecordDataFields(_existingDataRecordMock.Object)).Returns(_existingFieldData);
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                outcomeCode));
            _blaiseApiMock.Setup(b => b.UpdateCase(_existingDataRecordMock.Object, _newFieldData, 
                _instrumentName, _serverParkName));

            //act
            _sut.UpdateCase(_nisraDataRecordMock.Object, _existingDataRecordMock.Object, 
                _instrumentName, _serverParkName, outcomeCode);

            //assert
            _catiManaMock.Verify(v => v.RemoveCatiManaBlock(_newFieldData), Times.Once);
            _catiManaMock.Verify(v => v.RemoveCallHistoryBlock(_newFieldData), Times.Once);
            _catiManaMock.Verify(v => v.RemoveWebNudgedField(_newFieldData), Times.Once);
            _catiManaMock.Verify(v => v.AddCatiManaCallItems(_newFieldData, _existingFieldData,outcomeCode),
                Times.Once);

            _blaiseApiMock.Verify(v => v.UpdateCase(_existingDataRecordMock.Object, _newFieldData, 
                _instrumentName, _serverParkName), Times.Once);
        }
    }
}
