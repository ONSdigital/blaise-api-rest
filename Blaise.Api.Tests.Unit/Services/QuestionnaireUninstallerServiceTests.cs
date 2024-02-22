using System;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class QuestionnaireUninstallerServiceTests
    {
        private QuestionnaireUninstallerService _sut;

        private Mock<IBlaiseQuestionnaireApi> _blaiseQuestionnaireApiMock;
        private Mock<IBlaiseCaseApi> _blaiseCaseApiMock;
        private Mock<IBlaiseSqlApi> _blaiseSqlApiMock;

        private string _serverParkName;
        private string _questionnaireName;
        
        [SetUp]
        public void SetUpTests()
        {
            _blaiseQuestionnaireApiMock = new Mock<IBlaiseQuestionnaireApi>();
            _blaiseCaseApiMock = new Mock<IBlaiseCaseApi>();
            _blaiseSqlApiMock = new Mock<IBlaiseSqlApi>();

            _serverParkName = "ServerParkA";
            _questionnaireName = "OPN2010A";

            _sut = new QuestionnaireUninstallerService(
                _blaiseQuestionnaireApiMock.Object,
                _blaiseCaseApiMock.Object,
                _blaiseSqlApiMock.Object);
        }

        [Test]
        public void Given_I_Call_UninstallQuestionnaire_Then_The_Correct_Services_Are_Called()
        {
            //act
            _sut.UninstallQuestionnaire(_questionnaireName, _serverParkName);

            //assert
            _blaiseCaseApiMock.Verify(v => v.RemoveCases(_questionnaireName, _serverParkName));
            _blaiseQuestionnaireApiMock.Verify(v => v.UninstallQuestionnaire(_questionnaireName, _serverParkName, false, false, false)
                , Times.Once);
            _blaiseSqlApiMock.Verify(v => v.DropQuestionnaireTables(_questionnaireName)
                , Times.Once);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_UninstallQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UninstallQuestionnaire(string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_UninstallQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UninstallQuestionnaire(null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_UninstallQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UninstallQuestionnaire(_questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_UninstallQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UninstallQuestionnaire(_questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }
    }
}
