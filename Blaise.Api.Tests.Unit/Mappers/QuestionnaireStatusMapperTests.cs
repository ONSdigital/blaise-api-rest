namespace Blaise.Api.Tests.Unit.Mappers
{
    using System;
    using System.Collections.Generic;
    using Blaise.Api.Core.Mappers;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Blaise.Nuget.Api.Contracts.Extensions;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class QuestionnaireStatusMapperTests
    {
        private QuestionnaireStatusMapper _sut;
        private Mock<ISurvey> _surveyMock;

        private Mock<IConfiguration> _iConfiguration1Mock;
        private Mock<IConfiguration> _iConfiguration2Mock;
        private Mock<IConfiguration> _iConfiguration3Mock;

        [SetUp]
        public void SetupTests()
        {
            _surveyMock = new Mock<ISurvey>();

            _iConfiguration1Mock = new Mock<IConfiguration>();
            _iConfiguration2Mock = new Mock<IConfiguration>();
            _iConfiguration3Mock = new Mock<IConfiguration>();

            var iConfigurations = new List<IConfiguration>
            {
                _iConfiguration1Mock.Object,
                _iConfiguration2Mock.Object,
                _iConfiguration3Mock.Object,
            };

            _surveyMock.Setup(s => s.Configuration.Configurations).Returns(iConfigurations);
            _surveyMock.Setup(s => s.InstallDate).Returns(DateTime.Now);

            _sut = new QuestionnaireStatusMapper();
        }

        [Test]
        public void Given_A_Survey_Is_Active_Across_All_Nodes_When_I_Call_GetQuestionnaireStatus_Then_The_Active_Status_Is_Mapped()
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Active));
        }

        [Test]
        public void Given_A_Survey_Is_Inactive_Across_All_Nodes_When_I_Call_GetQuestionnaireStatus_Then_The_Inactive_Status_Is_Mapped()
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Inactive.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Inactive.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Inactive.FullName());

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Inactive));
        }

        [Test]
        public void Given_A_Survey_Is_Either_Active_Or_Installing_Across_All_Nodes_When_I_Call_GetQuestionnaireStatus_Then_The_Installing_Status_Is_Mapped()
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Installing.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Installing));
        }

        [TestCase(11)]
        [TestCase(60)]
        public void Given_A_Survey_Status_Is_Installing_But_Has_Taken_Too_Long_When_I_Call_GetQuestionnaireStatus_Then_The_Failed_Status_Is_Mapped(
            int minutesSpentInstalling)
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Installing.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());

            _surveyMock.Setup(s => s.InstallDate).Returns(DateTime.Now.AddMinutes(-minutesSpentInstalling));

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Failed));
        }

        [TestCase(10)]
        [TestCase(11)]
        [TestCase(60)]
        public void Given_A_Survey_Status_Is_Active_And_Is_Outside_Our_Expired_Installation_Period_When_I_Call_GetQuestionnaireStatus_Then_The_InstallDate_Is_Ignored_And_The_Active_Status_Is_Mapped(
            int minutesSpentInstalling)
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());

            _surveyMock.Setup(s => s.InstallDate).Returns(DateTime.Now.AddMinutes(-minutesSpentInstalling));

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Active));
        }

        [TestCase(QuestionnaireStatusType.Erroneous)]
        [TestCase(QuestionnaireStatusType.Other)]
        [TestCase(QuestionnaireStatusType.Inactive)]
        [TestCase(QuestionnaireStatusType.Failed)]
        public void Given_A_Survey_On_Any_Node_Has_Failed_When_I_Call_GetQuestionnaireStatus_Then_The_Failed_Status_Is_Mapped(
            QuestionnaireStatusType statusType)
        {
            // arrange
            _iConfiguration1Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Active.FullName());
            _iConfiguration2Mock.Setup(c => c.Status).Returns(statusType.FullName());
            _iConfiguration3Mock.Setup(c => c.Status).Returns(QuestionnaireStatusType.Installing.FullName());

            // act
            var result = _sut.GetQuestionnaireStatus(_surveyMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<QuestionnaireStatusType>());
            Assert.That(result, Is.EqualTo(QuestionnaireStatusType.Failed));
        }
    }
}
