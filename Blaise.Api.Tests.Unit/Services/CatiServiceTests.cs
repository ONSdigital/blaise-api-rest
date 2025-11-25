namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.Cati;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Api.Core.Services;
    using Blaise.Nuget.Api.Contracts.Exceptions;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Blaise.Nuget.Api.Contracts.Models;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class CatiServiceTests
    {
        private ICatiService _sut;
        private Mock<IBlaiseCatiApi> _blaiseCatiApiMock;
        private Mock<IBlaiseServerParkApi> _blaiseServerParkApiMock;
        private Mock<ICatiDtoMapper> _mapperMock;

        private CreateDayBatchDto _createDayBatchDto;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseCatiApiMock = new Mock<IBlaiseCatiApi>();
            _blaiseServerParkApiMock = new Mock<IBlaiseServerParkApi>();
            _mapperMock = new Mock<ICatiDtoMapper>();

            _createDayBatchDto = new CreateDayBatchDto { DayBatchDate = DateTime.Today };

            _sut = new CatiService(
                _blaiseCatiApiMock.Object,
                _blaiseServerParkApiMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetCatiQuestionnaires_Then_I_Get_A_List_Of_CatiQuestionnaireDto_Back()
        {
            // act
            var result = _sut.GetCatiQuestionnaires();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<CatiQuestionnaireDto>>());
        }

        [Test]
        public void Given_I_Call_GetCatiQuestionnaires_Then_I_Get_A_Correct_List_Of_CatiQuestionnaireDto_Returned()
        {
            // arrange
            const string ServerPark1 = "ServerParkA";
            const string ServerPark2 = "ServerParkB";
            var serverParkList = new List<string> { ServerPark1, ServerPark2 };

            const string Questionnaire1 = "OPN2010A";
            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(Questionnaire1);
            survey1Mock.Setup(s => s.ServerPark).Returns(ServerPark1);

            const string Questionnaire2 = "OPN2010B";
            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(Questionnaire2);
            survey2Mock.Setup(s => s.ServerPark).Returns(ServerPark2);

            _blaiseServerParkApiMock.Setup(b => b.GetNamesOfServerParks()).Returns(serverParkList);

            _blaiseCatiApiMock.Setup(bc => bc.GetInstalledQuestionnaires(ServerPark1)).Returns(new List<ISurvey> { survey1Mock.Object });
            _blaiseCatiApiMock.Setup(bc => bc.GetInstalledQuestionnaires(ServerPark2)).Returns(new List<ISurvey> { survey2Mock.Object });

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };
            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(Questionnaire1, ServerPark1))
                .Returns(surveyDays1);

            var surveyDays2 = new List<DateTime> { DateTime.Today };
            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(Questionnaire2, ServerPark2))
                .Returns(surveyDays2);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);

            var catiQuestionnaire2 = new CatiQuestionnaireDto { Name = "OPN2010B", SurveyDays = surveyDays2 };
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey2Mock.Object, surveyDays2))
                .Returns(catiQuestionnaire2);

            // act
            var result = _sut.GetCatiQuestionnaires().ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<CatiQuestionnaireDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(c => c.Name == Questionnaire1 && c.SurveyDays.Any(s => s == surveyDays1.First())), Is.True);
            Assert.That(result.Any(c => c.Name == Questionnaire2 && c.SurveyDays.Any(s => s == surveyDays2.First())), Is.True);
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaires_Then_I_Get_A_List_Of_CatiQuestionnaireDto_Back()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaires(ServerParkName)).Returns(new List<ISurvey>());

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<DateTime>());

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(It.IsAny<ISurvey>(), It.IsAny<List<DateTime>>()))
                .Returns(new CatiQuestionnaireDto());

            // act
            var result = _sut.GetCatiQuestionnaires(ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<CatiQuestionnaireDto>>());
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaires_Then_I_Get_A_Correct_List_Of_CatiQuestionnaireDto_Returned()
        {
            // arrange
            const string ServerPark = "ServerParkA";

            const string Questionnaire1 = "OPN2010A";
            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(Questionnaire1);
            survey1Mock.Setup(s => s.ServerPark).Returns(ServerPark);

            const string Questionnaire2 = "OPN2010B";
            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(Questionnaire2);
            survey2Mock.Setup(s => s.ServerPark).Returns(ServerPark);

            var surveyList = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object,
            };

            _blaiseCatiApiMock.Setup(b => b.GetInstalledQuestionnaires(ServerPark)).Returns(surveyList);

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };
            var surveyDays2 = new List<DateTime> { DateTime.Today };

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(Questionnaire1, ServerPark))
                .Returns(surveyDays1);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(Questionnaire2, ServerPark))
                .Returns(surveyDays2);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };
            var catiQuestionnaire2 = new CatiQuestionnaireDto { Name = "OPN2010B", SurveyDays = surveyDays2 };

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey2Mock.Object, surveyDays2))
                .Returns(catiQuestionnaire2);

            // act
            var result = _sut.GetCatiQuestionnaires(ServerPark).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<CatiQuestionnaireDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(c => c.Name == Questionnaire1 && c.SurveyDays.Any(s => s == surveyDays1.First())), Is.True);
            Assert.That(result.Any(c => c.Name == Questionnaire2 && c.SurveyDays.Any(s => s == surveyDays2.First())), Is.True);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCatiQuestionnaires_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaires(string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCatiQuestionnaires_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaires(null));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_Correct_Arguments_When_I_Call_GetCatiQuestionnaire_Then_I_Get_A_CatiQuestionnaireDto_Back()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(QuestionnaireName);
            survey1Mock.Setup(s => s.ServerPark).Returns(ServerParkName);

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaire(QuestionnaireName, ServerParkName))
                .Returns(survey1Mock.Object);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<DateTime>());

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(It.IsAny<ISurvey>(), It.IsAny<List<DateTime>>()))
                .Returns(new CatiQuestionnaireDto());

            // act
            var result = _sut.GetCatiQuestionnaire(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<CatiQuestionnaireDto>());
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaire_Then_I_Get_A_Correct_CatiQuestionnaireDto_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2010A";
            const string ServerParkName = "ServerParkA";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(QuestionnaireName);
            survey1Mock.Setup(s => s.ServerPark).Returns(ServerParkName);

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaire(QuestionnaireName, ServerParkName))
                .Returns(survey1Mock.Object);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(QuestionnaireName, ServerParkName))
                .Returns(surveyDays1);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);

            // act
            var result = _sut.GetCatiQuestionnaire(ServerParkName, QuestionnaireName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<CatiQuestionnaireDto>());
            Assert.That(result, Is.SameAs(catiQuestionnaire1));
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaire(ServerParkName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaire(ServerParkName, null));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaire(
                string.Empty,
                QuestionnaireName));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaire(null, QuestionnaireName));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_A_SurveyDay_Exists_When_I_Call_CreateDayBatch_Then_The_Correct_Service_Is_Called(bool checkForTreatedCases)
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _createDayBatchDto.CheckForTreatedCases = checkForTreatedCases;

            _blaiseCatiApiMock.Setup(b =>
                b.CreateDayBatch(QuestionnaireName, ServerParkName, (DateTime)_createDayBatchDto.DayBatchDate, checkForTreatedCases));

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            // act
            _sut.CreateDayBatch(QuestionnaireName, ServerParkName, _createDayBatchDto);

            // assert
            _blaiseCatiApiMock.Verify(
                v => v.CreateDayBatch(
                    QuestionnaireName,
                    ServerParkName,
                    (DateTime)_createDayBatchDto.DayBatchDate,
                    (bool)_createDayBatchDto.CheckForTreatedCases),
                Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_A_SurveyDay_Exists_When_I_Call_CreateDayBatch_Then_A_DayBatchDto_Is_Returned(bool checkForTreatedCases)
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _createDayBatchDto.CheckForTreatedCases = checkForTreatedCases;

            _blaiseCatiApiMock.Setup(b =>
                b.CreateDayBatch(QuestionnaireName, ServerParkName, (DateTime)_createDayBatchDto.DayBatchDate, checkForTreatedCases));

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            // act
            var result = _sut.CreateDayBatch(QuestionnaireName, ServerParkName, _createDayBatchDto);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<DayBatchDto>());
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateDayBatch(
                string.Empty,
                ServerParkName,
                _createDayBatchDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(
                null,
                ServerParkName,
                _createDayBatchDto));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateDayBatch(
                QuestionnaireName,
                string.Empty,
                _createDayBatchDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(
                QuestionnaireName,
                null,
                _createDayBatchDto));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_A_Null_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(
                QuestionnaireName,
                ServerParkName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("The argument 'createDayBatchDto' must be supplied"));
        }

        [Test]
        public void Given_A_Null_DayBatchDate_In_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var createDayBatchDto = new CreateDayBatchDto { CheckForTreatedCases = true };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(
                QuestionnaireName,
                ServerParkName,
                createDayBatchDto));
            Assert.That(exception.ParamName, Is.EqualTo("The argument 'createDayBatchDto.DayBatchDate' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CheckForTreatedCases_In_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var createDayBatchDto = new CreateDayBatchDto { DayBatchDate = DateTime.Today };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(
                QuestionnaireName,
                ServerParkName,
                createDayBatchDto));
            Assert.That(exception.ParamName, Is.EqualTo("The argument 'createDayBatchDto.CheckForTreatedCases' must be supplied"));
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_GetDayBatch_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns(new DayBatchModel());

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            // act
            _sut.GetDayBatch(QuestionnaireName, ServerParkName);

            // assert
            _blaiseCatiApiMock.Verify(v => v.GetDayBatch(QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_GetDayBatch_Then_A_DayBatchDto_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns(new DayBatchModel());

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            // act
            var result = _sut.GetDayBatch(QuestionnaireName, ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<DayBatchDto>());
        }

        [Test]
        public void Given_A_DayBatch_Does_Not_Exist_When_I_Call_GetDayBatch_Then_A_DataNotFoundException_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns((DayBatchModel)null);

            // act and assert
            Assert.Throws<DataNotFoundException>(() => _sut.GetDayBatch(QuestionnaireName, ServerParkName));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_GetDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetDayBatch(string.Empty, ServerParkName));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_GetDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetDayBatch(null, ServerParkName));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetDayBatch(QuestionnaireName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetDayBatch(QuestionnaireName, null));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_A_DayBatch_Exists_Today_When_I_Call_QuestionnaireHasADayBatchForToday_Then_True_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var dayBatchModel = new DayBatchModel { DayBatchDate = DateTime.Today };

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns(dayBatchModel);

            // act
            var result = _sut.QuestionnaireHasADayBatchForToday(QuestionnaireName, ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.True);
        }

        [Test]
        public void Given_A_DayBatch_Does_Not_Exist_Today_When_I_Call_QuestionnaireHasADayBatchForToday_Then_False_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var dayBatchModel = new DayBatchModel { DayBatchDate = DateTime.Today.AddDays(-1) };

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns(dayBatchModel);

            // act
            var result = _sut.QuestionnaireHasADayBatchForToday(QuestionnaireName, ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Given_An_Questionnaire_Has_No_DayBatch_When_I_Call_QuestionnaireHasADayBatchForToday_Then_False_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(QuestionnaireName, ServerParkName)).Returns((DayBatchModel)null);

            // act
            var result = _sut.QuestionnaireHasADayBatchForToday(QuestionnaireName, ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.False);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireHasADayBatchForToday(string.Empty, ServerParkName));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireHasADayBatchForToday(null, ServerParkName));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireHasADayBatchForToday(QuestionnaireName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireHasADayBatchForToday(QuestionnaireName, null));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_AddCasesToDayBatch_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002",
            };

            // act
            _sut.AddCasesToDayBatch(QuestionnaireName, ServerParkName, caseIds);

            // assert
            _blaiseCatiApiMock.Verify(v => v.AddToDayBatch(QuestionnaireName, ServerParkName, "1000001"), Times.Once);
            _blaiseCatiApiMock.Verify(v => v.AddToDayBatch(QuestionnaireName, ServerParkName, "1000002"), Times.Once);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(string.Empty, ServerParkName, caseIds));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(null, ServerParkName, caseIds));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(QuestionnaireName, string.Empty, caseIds));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002",
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(QuestionnaireName, null, caseIds));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_An_Empty_List_Of_CaseIds_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(QuestionnaireName, ServerParkName, new List<string>()));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'caseIds' must be supplied"));
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_AddCaseToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(QuestionnaireName, ServerParkName, null));
            Assert.That(exception.ParamName, Is.EqualTo("caseIds"));
        }

        [Test]
        public void Given_SurveyDays_Exist_When_I_Call_GetSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(QuestionnaireName, ServerParkName)).Returns(It.IsAny<List<DateTime>>());

            // act
            _sut.GetSurveyDays(QuestionnaireName, ServerParkName);

            // assert
            _blaiseCatiApiMock.Verify(v => v.GetSurveyDays(QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_SurveyDays_Exist_When_I_Call_GetSurveyDays_Then_A_Correct_SurveyDaysDto_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1),
            };

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(QuestionnaireName, ServerParkName)).Returns(surveyDays);

            // act
            var result = _sut.GetSurveyDays(QuestionnaireName, ServerParkName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<DateTime>>());
            Assert.That(result, Has.Member(DateTime.Today));
            Assert.That(result, Has.Member(DateTime.Today.AddDays(1)));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_GetSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetSurveyDays(string.Empty, ServerParkName));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_GetSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetSurveyDays(null, ServerParkName));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetSurveyDays(QuestionnaireName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetSurveyDays(QuestionnaireName, null));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1),
            };

            _blaiseCatiApiMock.Setup(b =>
                b.SetSurveyDays(QuestionnaireName, ServerParkName, surveyDays));

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(QuestionnaireName, ServerParkName)).Returns(surveyDays);

            // act
            _sut.AddSurveyDays(QuestionnaireName, ServerParkName, surveyDays);

            // assert
            _blaiseCatiApiMock.Verify(
                v => v.SetSurveyDays(
                    QuestionnaireName,
                    ServerParkName,
                    surveyDays),
                Times.Once);

            _blaiseCatiApiMock.Verify(v => v.GetSurveyDays(QuestionnaireName, ServerParkName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddSurveyDays_Then_A_Correct_SurveyDaysDto_Is_Returned()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1),
            };

            _blaiseCatiApiMock.Setup(b =>
                b.SetSurveyDays(QuestionnaireName, ServerParkName, surveyDays));

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(QuestionnaireName, ServerParkName)).Returns(surveyDays);

            // act
            var result = _sut.AddSurveyDays(QuestionnaireName, ServerParkName, surveyDays);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<DateTime>>());
            Assert.That(result, Has.Member(DateTime.Today));
            Assert.That(result, Has.Member(DateTime.Today.AddDays(1)));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(
                string.Empty,
                ServerParkName,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(
                null,
                ServerParkName,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(
                QuestionnaireName,
                string.Empty,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(
                QuestionnaireName,
                null,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_A_Null_List_Of_SurveyDays_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(
                QuestionnaireName,
                ServerParkName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("surveyDays"));
        }

        [Test]
        public void Given_An_Empty_SurveyDays_List_In_AddSurveyDaysDto_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var surveyDays = new List<DateTime>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(
                QuestionnaireName,
                ServerParkName,
                surveyDays));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'surveyDays' must be supplied"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_RemoveSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1),
            };

            // act
            _sut.RemoveSurveyDays(QuestionnaireName, ServerParkName, surveyDays);

            // assert
            _blaiseCatiApiMock.Verify(
                v => v.RemoveSurveyDays(
                    QuestionnaireName,
                    ServerParkName,
                    surveyDays),
                Times.Once);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(
                string.Empty,
                ServerParkName,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'questionnaireName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(
                null,
                ServerParkName,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.ParamName, Is.EqualTo("questionnaireName"));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(
                QuestionnaireName,
                string.Empty,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'serverParkName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(
                QuestionnaireName,
                null,
                It.IsAny<List<DateTime>>()));
            Assert.That(exception.ParamName, Is.EqualTo("serverParkName"));
        }

        [Test]
        public void Given_A_Null_List_Of_SurveyDays_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(
                QuestionnaireName,
                ServerParkName,
                null));
            Assert.That(exception.ParamName, Is.EqualTo("surveyDays"));
        }

        [Test]
        public void Given_An_Empty_SurveyDays_List_In_AddSurveyDaysDto_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            const string QuestionnaireName = "OPN2101A";
            const string ServerParkName = "ServerParkA";
            var surveyDays = new List<DateTime>();

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(
                QuestionnaireName,
                ServerParkName,
                surveyDays));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'surveyDays' must be supplied"));
        }
    }
}
