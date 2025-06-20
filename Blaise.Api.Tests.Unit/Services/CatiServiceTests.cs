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

namespace Blaise.Api.Tests.Unit.Services
{
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
            //act
            var result = _sut.GetCatiQuestionnaires();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<CatiQuestionnaireDto>>(result);
        }

        [Test]
        public void Given_I_Call_GetCatiQuestionnaires_Then_I_Get_A_Correct_List_Of_CatiQuestionnaireDto_Returned()
        {
            //arrange
            const string serverPark1 = "ServerParkA";
            const string serverPark2 = "ServerParkB";
            var serverParkList = new List<string> { serverPark1, serverPark2 };

            const string questionnaire1 = "OPN2010A";
            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark1);

            const string questionnaire2 = "OPN2010B";
            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(questionnaire2);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark2);


            _blaiseServerParkApiMock.Setup(b => b.GetNamesOfServerParks()).Returns(serverParkList);

            _blaiseCatiApiMock.Setup(bc => bc.GetInstalledQuestionnaires(serverPark1)).Returns(new List<ISurvey> { survey1Mock.Object });
            _blaiseCatiApiMock.Setup(bc => bc.GetInstalledQuestionnaires(serverPark2)).Returns(new List<ISurvey> { survey2Mock.Object });

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };
            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaire1, serverPark1))
                .Returns(surveyDays1);

            var surveyDays2 = new List<DateTime> { DateTime.Today };
            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaire2, serverPark2))
                .Returns(surveyDays2);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);

            var catiQuestionnaire2 = new CatiQuestionnaireDto { Name = "OPN2010B", SurveyDays = surveyDays2 };
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey2Mock.Object, surveyDays2))
                .Returns(catiQuestionnaire2);

            //act
            var result = _sut.GetCatiQuestionnaires().ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<CatiQuestionnaireDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
            Assert.True(result.Any(c => c.Name == questionnaire1 && c.SurveyDays.Any(s => s == surveyDays1.First())));
            Assert.True(result.Any(c => c.Name == questionnaire2 && c.SurveyDays.Any(s => s == surveyDays2.First())));
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaires_Then_I_Get_A_List_Of_CatiQuestionnaireDto_Back()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaires(serverParkName)).Returns(new List<ISurvey>());

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<DateTime>());

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(It.IsAny<ISurvey>(), It.IsAny<List<DateTime>>()))
                .Returns(new CatiQuestionnaireDto());

            //act
            var result = _sut.GetCatiQuestionnaires(serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<CatiQuestionnaireDto>>(result);
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaires_Then_I_Get_A_Correct_List_Of_CatiQuestionnaireDto_Returned()
        {
            //arrange
            const string serverPark = "ServerParkA";

            const string questionnaire1 = "OPN2010A";
            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaire1);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverPark);

            const string questionnaire2 = "OPN2010B";
            var survey2Mock = new Mock<ISurvey>();
            survey2Mock.Setup(s => s.Name).Returns(questionnaire2);
            survey2Mock.Setup(s => s.ServerPark).Returns(serverPark);

            var surveyList = new List<ISurvey>
            {
                survey1Mock.Object,
                survey2Mock.Object
            };

            _blaiseCatiApiMock.Setup(b => b.GetInstalledQuestionnaires(serverPark)).Returns(surveyList);

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };
            var surveyDays2 = new List<DateTime> { DateTime.Today };

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaire1, serverPark))
                .Returns(surveyDays1);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaire2, serverPark))
                .Returns(surveyDays2);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };
            var catiQuestionnaire2 = new CatiQuestionnaireDto { Name = "OPN2010B", SurveyDays = surveyDays2 };

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);
            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey2Mock.Object, surveyDays2))
                .Returns(catiQuestionnaire2);

            //act
            var result = _sut.GetCatiQuestionnaires(serverPark).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<CatiQuestionnaireDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);
            Assert.True(result.Any(c => c.Name == questionnaire1 && c.SurveyDays.Any(s => s == surveyDays1.First())));
            Assert.True(result.Any(c => c.Name == questionnaire2 && c.SurveyDays.Any(s => s == surveyDays2.First())));
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCatiQuestionnaires_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaires(string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCatiQuestionnaires_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaires(null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_Correct_Arguments_When_I_Call_GetCatiQuestionnaire_Then_I_Get_A_CatiQuestionnaireDto_Back()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaireName);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverParkName);

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaire(questionnaireName, serverParkName))
                .Returns(survey1Mock.Object);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<DateTime>());

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(It.IsAny<ISurvey>(), It.IsAny<List<DateTime>>()))
                .Returns(new CatiQuestionnaireDto());

            //act
            var result = _sut.GetCatiQuestionnaire(serverParkName, questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
        }

        [Test]
        public void Given_A_ServerPark_When_I_Call_GetCatiQuestionnaire_Then_I_Get_A_Correct_CatiQuestionnaireDto_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2010A";
            const string serverParkName = "ServerParkA";

            var survey1Mock = new Mock<ISurvey>();
            survey1Mock.Setup(s => s.Name).Returns(questionnaireName);
            survey1Mock.Setup(s => s.ServerPark).Returns(serverParkName);

            var surveyDays1 = new List<DateTime> { DateTime.Today.AddDays(-1) };

            _blaiseCatiApiMock.Setup(i => i.GetInstalledQuestionnaire(questionnaireName, serverParkName))
                .Returns(survey1Mock.Object);

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaireName, serverParkName))
                .Returns(surveyDays1);

            var catiQuestionnaire1 = new CatiQuestionnaireDto { Name = "OPN2010A", SurveyDays = surveyDays1 };

            _mapperMock.Setup(m => m.MapToCatiQuestionnaireDto(survey1Mock.Object, surveyDays1))
                .Returns(catiQuestionnaire1);

            //act
            var result = _sut.GetCatiQuestionnaire(serverParkName, questionnaireName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<CatiQuestionnaireDto>(result);
            Assert.AreSame(catiQuestionnaire1, result);
        }


        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaire(serverParkName, string.Empty));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaire(serverParkName, null));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            var questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetCatiQuestionnaire(string.Empty,
                questionnaireName));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetCatiQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetCatiQuestionnaire(null, questionnaireName));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_A_SurveyDay_Exists_When_I_Call_CreateDayBatch_Then_The_Correct_Service_Is_Called(bool checkForTreatedCases)
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _createDayBatchDto.CheckForTreatedCases = checkForTreatedCases;

            _blaiseCatiApiMock.Setup(b =>
                b.CreateDayBatch(questionnaireName, serverParkName, (DateTime)_createDayBatchDto.DayBatchDate, checkForTreatedCases));

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            //act
            _sut.CreateDayBatch(questionnaireName, serverParkName, _createDayBatchDto);

            //assert
            _blaiseCatiApiMock.Verify(v => v.CreateDayBatch(questionnaireName, serverParkName,
                (DateTime)_createDayBatchDto.DayBatchDate, (bool)_createDayBatchDto.CheckForTreatedCases), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_A_SurveyDay_Exists_When_I_Call_CreateDayBatch_Then_A_DayBatchDto_Is_Returned(bool checkForTreatedCases)
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _createDayBatchDto.CheckForTreatedCases = checkForTreatedCases;

            _blaiseCatiApiMock.Setup(b =>
                b.CreateDayBatch(questionnaireName, serverParkName, (DateTime)_createDayBatchDto.DayBatchDate, checkForTreatedCases));

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            //act
            var result = _sut.CreateDayBatch(questionnaireName, serverParkName, _createDayBatchDto);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DayBatchDto>(result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_CreateDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateDayBatch(string.Empty,
                serverParkName, _createDayBatchDto));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(null,
                serverParkName, _createDayBatchDto));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_CreateDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.CreateDayBatch(questionnaireName,
                string.Empty, _createDayBatchDto));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(questionnaireName,
                null, _createDayBatchDto));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(questionnaireName,
                serverParkName, null));
            Assert.AreEqual("The argument 'createDayBatchDto' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_DayBatchDate_In_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var createDayBatchDto = new CreateDayBatchDto { CheckForTreatedCases = true };


            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(questionnaireName,
                serverParkName, createDayBatchDto));
            Assert.AreEqual("The argument 'createDayBatchDto.DayBatchDate' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_CheckForTreatedCases_In_CreateDayBatchDto_When_I_Call_CreateDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var createDayBatchDto = new CreateDayBatchDto { DayBatchDate = DateTime.Today };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.CreateDayBatch(questionnaireName,
                serverParkName, createDayBatchDto));
            Assert.AreEqual("The argument 'createDayBatchDto.CheckForTreatedCases' must be supplied", exception.ParamName);
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_GetDayBatch_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns(new DayBatchModel());

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            //act
            _sut.GetDayBatch(questionnaireName, serverParkName);

            //assert
            _blaiseCatiApiMock.Verify(v => v.GetDayBatch(questionnaireName, serverParkName), Times.Once);
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_GetDayBatch_Then_A_DayBatchDto_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns(new DayBatchModel());

            _mapperMock.Setup(m => m.MapToDayBatchDto(It.IsAny<DayBatchModel>()))
                .Returns(new DayBatchDto());

            //act
            var result = _sut.GetDayBatch(questionnaireName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<DayBatchDto>(result);
        }

        [Test]
        public void Given_A_DayBatch_Does_Not_Exist_When_I_Call_GetDayBatch_Then_A_DataNotFoundException_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns((DayBatchModel)null);

            //act && assert
            Assert.Throws<DataNotFoundException>(() => _sut.GetDayBatch(questionnaireName, serverParkName));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_GetDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetDayBatch(string.Empty, serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_GetDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetDayBatch(null, serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetDayBatch(questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetDayBatch(questionnaireName, null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_DayBatch_Exists_Today_When_I_Call_QuestionnaireHasADayBatchForToday_Then_True_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var dayBatchModel = new DayBatchModel { DayBatchDate = DateTime.Today };

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns(dayBatchModel);

            //act
            var result = _sut.QuestionnaireHasADayBatchForToday(questionnaireName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_DayBatch_Does_Not_Exist_Today_When_I_Call_QuestionnaireHasADayBatchForToday_Then_False_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var dayBatchModel = new DayBatchModel { DayBatchDate = DateTime.Today.AddDays(-1) };

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns(dayBatchModel);

            //act
            var result = _sut.QuestionnaireHasADayBatchForToday(questionnaireName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_An_Questionnaire_Has_No_DayBatch_When_I_Call_QuestionnaireHasADayBatchForToday_Then_False_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetDayBatch(questionnaireName, serverParkName)).Returns((DayBatchModel)null);

            //act
            var result = _sut.QuestionnaireHasADayBatchForToday(questionnaireName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireHasADayBatchForToday(string.Empty, serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireHasADayBatchForToday(null, serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireHasADayBatchForToday(questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_QuestionnaireHasADayBatchForToday_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireHasADayBatchForToday(questionnaireName, null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_DayBatch_Exists_When_I_Call_AddCasesToDayBatch_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002"
            };

            //act
            _sut.AddCasesToDayBatch(questionnaireName, serverParkName, caseIds);

            //assert
            _blaiseCatiApiMock.Verify(v => v.AddToDayBatch(questionnaireName, serverParkName, "1000001"), Times.Once);
            _blaiseCatiApiMock.Verify(v => v.AddToDayBatch(questionnaireName, serverParkName, "1000002"), Times.Once);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(string.Empty, serverParkName, caseIds));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(null, serverParkName, caseIds));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(questionnaireName, string.Empty, caseIds));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            var caseIds = new List<string>
            {
                "1000001",
                "1000002"
            };

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(questionnaireName, null, caseIds));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_List_Of_CaseIds_When_I_Call_AddCasesToDayBatch_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddCasesToDayBatch(questionnaireName, serverParkName, new List<string>()));
            Assert.AreEqual("A value for the argument 'caseIds' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_CaseId_When_I_Call_AddCaseToDayBatch_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddCasesToDayBatch(questionnaireName, serverParkName, null));
            Assert.AreEqual("caseIds", exception.ParamName);
        }

        [Test]
        public void Given_SurveyDays_Exist_When_I_Call_GetSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaireName, serverParkName)).Returns(It.IsAny<List<DateTime>>());

            //act
            _sut.GetSurveyDays(questionnaireName, serverParkName);

            //assert
            _blaiseCatiApiMock.Verify(v => v.GetSurveyDays(questionnaireName, serverParkName), Times.Once);
        }

        [Test]
        public void Given_SurveyDays_Exist_When_I_Call_GetSurveyDays_Then_A_Correct_SurveyDaysDto_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1)
            };

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaireName, serverParkName)).Returns(surveyDays);

            //act
            var result = _sut.GetSurveyDays(questionnaireName, serverParkName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<DateTime>>(result);
            Assert.IsTrue(result.Contains(DateTime.Today));
            Assert.IsTrue(result.Contains(DateTime.Today.AddDays(1)));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_GetSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetSurveyDays(string.Empty, serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_GetSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetSurveyDays(null, serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetSurveyDays(questionnaireName, string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetSurveyDays(questionnaireName, null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1)
            };

            _blaiseCatiApiMock.Setup(b =>
                b.SetSurveyDays(questionnaireName, serverParkName, surveyDays));

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaireName, serverParkName)).Returns(surveyDays);

            //act
            _sut.AddSurveyDays(questionnaireName, serverParkName, surveyDays);

            //assert
            _blaiseCatiApiMock.Verify(v => v.SetSurveyDays(questionnaireName, serverParkName,
                surveyDays), Times.Once);

            _blaiseCatiApiMock.Verify(v => v.GetSurveyDays(questionnaireName, serverParkName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddSurveyDays_Then_A_Correct_SurveyDaysDto_Is_Returned()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1)
            };

            _blaiseCatiApiMock.Setup(b =>
                b.SetSurveyDays(questionnaireName, serverParkName, surveyDays));

            _blaiseCatiApiMock.Setup(b => b.GetSurveyDays(questionnaireName, serverParkName)).Returns(surveyDays);

            //act
            var result = _sut.AddSurveyDays(questionnaireName, serverParkName, surveyDays);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<DateTime>>(result);
            Assert.IsTrue(result.Contains(DateTime.Today));
            Assert.IsTrue(result.Contains(DateTime.Today.AddDays(1)));
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(string.Empty,
                serverParkName, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(null,
                serverParkName, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(questionnaireName,
                string.Empty, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(questionnaireName,
                null, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_List_Of_SurveyDays_When_I_Call_AddSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddSurveyDays(questionnaireName,
                serverParkName, null));
            Assert.AreEqual("surveyDays", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_SurveyDays_List_In_AddSurveyDaysDto_When_I_Call_AddSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var surveyDays = new List<DateTime>();


            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddSurveyDays(questionnaireName,
                serverParkName, surveyDays));
            Assert.AreEqual("A value for the argument 'surveyDays' must be supplied", exception.Message);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_RemoveSurveyDays_Then_The_Correct_Service_Is_Called()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            var surveyDays = new List<DateTime>
            {
                DateTime.Today,
                DateTime.Today.AddDays(1)
            };

            //act
            _sut.RemoveSurveyDays(questionnaireName, serverParkName, surveyDays);

            //assert
            _blaiseCatiApiMock.Verify(v => v.RemoveSurveyDays(questionnaireName, serverParkName,
                surveyDays), Times.Once);
        }

        [Test]
        public void Given_An_Empty_questionnaireName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(string.Empty,
                serverParkName, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_questionnaireName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(null,
                serverParkName, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(questionnaireName,
                string.Empty, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(questionnaireName,
                null, It.IsAny<List<DateTime>>()));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Null_List_Of_SurveyDays_When_I_Call_RemoveSurveyDays_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveSurveyDays(questionnaireName,
                serverParkName, null));
            Assert.AreEqual("surveyDays", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_SurveyDays_List_In_AddSurveyDaysDto_When_I_Call_RemoveSurveyDays_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var surveyDays = new List<DateTime>();


            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveSurveyDays(questionnaireName,
                serverParkName, surveyDays));
            Assert.AreEqual("A value for the argument 'surveyDays' must be supplied", exception.Message);
        }
    }
}
