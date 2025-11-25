namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Globalization;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Core.Services;
    using Blaise.Api.Tests.Unit.Helpers;
    using Blaise.Nuget.Api.Contracts.Models;
    using Moq;
    using NUnit.Framework;

    public class NisraCaseComparisonServiceTests
    {
        private readonly string _questionnaireName = "OPN2101A";
        private readonly string _primaryKey = "900000";
        private readonly int _hOutComplete = 110;
        private readonly int _hOutPartial = 210;
        private readonly int _hOutIneligible = 580;
        private readonly int _hOutNotStarted = 0;
        private readonly string _date1 = DateTime.Now.AddHours(-1).ToString(CultureInfo.InvariantCulture);
        private readonly string _date2 = DateTime.Now.AddHours(-2).ToString(CultureInfo.InvariantCulture);
        private Mock<ILoggingService> _loggingMock;
        private NisraCaseComparisonService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggingMock = new Mock<ILoggingService>();

            _sut = new NisraCaseComparisonService(_loggingMock.Object);
        }

        // Scenario 1
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Complete_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 2
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Complete_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 3
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Partial_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 4
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Between_210_And_542_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned(int existingOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, existingOutcome, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 5 & 8
        [TestCase(110)]
        [TestCase(310)]
        public void Given_The_Nisra_Outcome_Is_Zero_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned(int existingOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutNotStarted, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, existingOutcome, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 6
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Partial_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 7
        [TestCase(210)]
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Between_210_And_542_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned(int existingOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, existingOutcome, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 8

        // Additional scenario
        [TestCase(110)]
        [TestCase(210)]
        public void Given_The_Nisra_Case_Has_A_Valid_Outcome_But_Existing_Case_Has_An_Outcome_Of_Zero_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned(int nisraOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutNotStarted, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 9
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned()
        {
            // arrange
            const int OutcomeCode = 562; // respondent request for data to be deleted

            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, OutcomeCode, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 10
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Delete_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned()
        {
            // arrange
            const int OutcomeCode = 561; // respondent request for data to be deleted

            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, OutcomeCode, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 11 - See case service tests

        // Scenario 12
        [Test]
        public void Given_The_Case_Has_Been_Processed_Before_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 13
        [Test]
        public void Given_The_Nisra_Case_And_Existing_Case_Have_An_Outcome_Of_Ineligible_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 14
        [TestCase(310)]
        [TestCase(430)]
        [TestCase(440)]
        [TestCase(460)]
        [TestCase(461)]
        [TestCase(541)]
        [TestCase(542)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Ineligible_And_Existing_Case_Has_An_Outcome_Between_310_And_542_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned(int existingOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, existingOutcome, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 15
        [TestCase(110)]
        [TestCase(210)]
        [TestCase(561)]
        [TestCase(562)]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Ineligible_And_Existing_Case_Has_An_Outcome_Of_110_210_561_Or_562_When_I_Call_CaseNeedsToBeUpdated_Then_False_Is_Returned(int existingOutcome)
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, existingOutcome, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.False);
        }

        // Scenario 16
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Complete_And_Existing_Case_Has_An_Outcome_Of_Ineligible_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        // Scenario 17
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Partial_And_Existing_Case_Has_An_Outcome_Of_Ineligible_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Given_A_Record_Has_Been_Processed_Before_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);

            // act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(
                nisraCaseStatusModel,
                existingCaseStatusModel,
                _questionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Given_A_Record_Has_Not_Been_Processed_Before_Due_To_Different_Outcome_Codes_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_False_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutPartial, _date1);

            // act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(
                nisraCaseStatusModel,
                existingCaseStatusModel,
                _questionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.False);
        }

        // Scenario 18
        [Test]
        public void Given_The_Nisra_Case_Has_An_Outcome_Of_Inelgible_And_Existing_Case_Has_An_Outcome_Of_Not_Started_When_I_Call_CaseNeedsToBeUpdated_Then_True_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutIneligible, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutNotStarted, _date2);

            // act
            var result = _sut.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, _questionnaireName);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Given_A_Record_Has_Not_Been_Processed_Before_Due_To_Different_LastUpdated_Dates_When_I_Call_NisraRecordHasAlreadyBeenProcessed_Then_False_Is_Returned()
        {
            // arrange
            var primaryKeyValues = PrimaryKeyHelper.CreatePrimaryKeys(_primaryKey);
            var nisraCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date1);
            var existingCaseStatusModel = new CaseStatusModel(primaryKeyValues, _hOutComplete, _date2);

            // act
            var result = _sut.NisraRecordHasAlreadyBeenProcessed(
                nisraCaseStatusModel,
                existingCaseStatusModel,
                _questionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.False);
        }
    }
}
