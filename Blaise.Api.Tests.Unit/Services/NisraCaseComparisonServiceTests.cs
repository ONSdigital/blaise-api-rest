using System;
using System.Globalization;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class NisraCaseComparisonServiceTests
    {
        private Mock<ILoggingService> _loggingMock;

        private readonly string _instrumentName;
        private readonly string _primaryKey;
        private readonly int _hOutComplete;
        private readonly int _hOutPartial;
        private readonly int _hOutNotStarted;
        private readonly string _date1;
        private readonly string _date2;

        private NisraCaseComparisonService _sut;
        
        public NisraCaseComparisonServiceTests()
        {
            _instrumentName = "OPN2101A";
            _primaryKey = "900000";
            _hOutComplete = 110;
            _hOutPartial = 210;
            _hOutNotStarted = 0;

            _date1 = DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture);
            _date2 = DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture);
        }

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILoggingService>();

            _sut = new NisraCaseComparisonService(_loggingMock.Object);
        }
        
        // Scenario 1 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Complete_When_I_Call_UpdateExistingCase_Then_True_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 2 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Complete_When_I_Call_UpdateExistingCase_Then_False_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsFalse(result);
        }

        // Scenario 3 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Partial_When_I_Call_UpdateExistingCase_Then_True_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 4  (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Between_210_And_542_When_I_Call_UpdateExistingCase_Then_True_Is_Returned(int existingOutcome)
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, existingOutcome, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 5 & 8 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(110)]
        [TestCase(310)]
        public void Given_The_Nisra_Outcome_Is_Zero_When_I_Call_UpdateExistingCase_Then_False_Is_Returned(int existingOutcome)
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutNotStarted, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, existingOutcome, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsFalse(result);
        }

        // Scenario 6 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Partial_When_I_Call_UpdateExistingCase_Then_True_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 7 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Haw_An_Outcome_Between_210_And_542_When_I_Call_UpdateExistingCase_Then_True_Is_Returned(int existingOutcome)
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, existingOutcome, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 8 - covered by Scenario 5 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)

        //additional scenario
        [TestCase(110)]
        [TestCase(210)]
        public void Given_The_Nisra_Case_Has_A_Valid_Outcome_But_Existing_Case_Has_An_Outcome_Of_Zero_When_I_Call_UpdateExistingCase_Then_True_Is_Returned(int nisraOutcome)
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutNotStarted, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsTrue(result);
        }

        // Scenario 9 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_UpdateExistingCase_Then_False_Is_Returned()
        {
            //arrange
            const int outcomeCode = 562; //respondent request for data to be deleted

            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, outcomeCode, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsFalse(result);
        }

        // Scenario 10 (https://collaborate2.ons.gov.uk/confluence/display/QSS/Blaise+5+NISRA+Case+Processor+Flow)
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_UpdateExistingCase_Then_False_Is_Returned()
        {
            //arrange
            const int outcomeCode = 561; //respondent request for data to be deleted

            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, outcomeCode, _date2);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsFalse(result);
        }

        // Scenario 11
        // see case service tests

        // Scenario 12
        [Test]
        public void Given_The_Case_Has_Been_Processed_Before_When_I_Call_UpdateExistingCase_Then_False_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);

            //act
            var result = _sut.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel, _instrumentName);

            //assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_A_Record_Has_Been_Processed_Before_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_True_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);

            //act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(nisraCaseStatusModel, existingCaseStatusModel,
                _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.True(result);
        }

        [Test]
        public void Given_A_Record_Has_Not_Been_Processed_Before_Due_To_Different_Outcome_Codes_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_False_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutPartial, _date1);

            //act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(nisraCaseStatusModel, existingCaseStatusModel,
                _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.False(result);
        }

        [Test]
        public void Given_A_Record_Has_Not_Been_Processed_Before_Due_To_Different_LastUpdated_Dates_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_False_Is_Returned()
        {
            //arrange
            var nisraCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(_primaryKey, _hOutComplete, _date2);

            //act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(nisraCaseStatusModel, existingCaseStatusModel,
                _instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.False(result);
        }
    }
}
