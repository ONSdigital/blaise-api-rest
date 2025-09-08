namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Api.Core.Services;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Blaise.Nuget.Api.Contracts.Models;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class QuestionnaireServiceTests
    {
        private IQuestionnaireService _sut;

        private Mock<IBlaiseQuestionnaireApi> _blaiseApiMock;
        private Mock<IQuestionnaireDtoMapper> _questionnaireMapperMock;
        private Mock<IDataEntrySettingsDtoMapper> _dataEntrySettingsMapperMock;
        private string _questionnaireName;
        private string _serverParkName;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseQuestionnaireApi>();
            _questionnaireMapperMock = new Mock<IQuestionnaireDtoMapper>();
            _dataEntrySettingsMapperMock = new Mock<IDataEntrySettingsDtoMapper>();

            _questionnaireName = "OPN2101A";
            _serverParkName = "ServerParkA";

            _sut = new QuestionnaireService(
                _blaiseApiMock.Object,
                _questionnaireMapperMock.Object,
                _dataEntrySettingsMapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetAllQuestionnaires_Then_I_Get_A_List_Of_QuestionnaireDtos_Returned()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetQuestionnairesAcrossServerParks())
                .Returns(new List<ISurvey>());

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDtos(new List<ISurvey>()))
                .Returns(new List<QuestionnaireDto>());

            // act
            var result = _sut.GetAllQuestionnaires();

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<QuestionnaireDto>>(result);
        }

        [Test]
        public void Given_I_Call_GetAllQuestionnaires_Then_I_Get_A_List_Of_All_Questionnaires_Across_All_ServerParks()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetQuestionnairesAcrossServerParks())
                .Returns(new List<ISurvey>());

            // act
            _sut.GetAllQuestionnaires();

            // assert
            _blaiseApiMock.Verify(b => b.GetQuestionnairesAcrossServerParks());
        }

        [Test]
        public void Given_I_Call_GetAllQuestionnaires_Then_I_Get_A_Correct_List_Of_QuestionnaireDtos_Returned()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetQuestionnairesAcrossServerParks())
                .Returns(new List<ISurvey>());

            var questionnaireDtos = new List<QuestionnaireDto>
            {
                new QuestionnaireDto { Name = "OPN2010A" },
                new QuestionnaireDto { Name = "OPN2010B" },
            };

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDtos(new List<ISurvey>()))
                .Returns(questionnaireDtos);

            // act
            var result = _sut.GetAllQuestionnaires().ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(questionnaireDtos, result);
        }

        [Test]
        public void Given_I_Call_GetQuestionnaires_Then_I_Get_A_List_Of_QuestionnaireDtos_Back()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetQuestionnaires(_serverParkName))
                .Returns(new List<ISurvey>());

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDtos(new List<ISurvey>()))
                .Returns(new List<QuestionnaireDto>());

            // act
            var result = _sut.GetQuestionnaires(_serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<QuestionnaireDto>>(result);
        }

        [Test]
        public void Given_I_Call_GetQuestionnaire_Then_I_Get_A_Correct_List_Of_QuestionnaireDtos_Back()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetQuestionnaires(_serverParkName))
                .Returns(new List<ISurvey>());

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDtos(new List<ISurvey>()))
                .Returns(new List<QuestionnaireDto>());

            // act
            var result = _sut.GetQuestionnaires(_serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(new List<QuestionnaireDto>(), result);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetQuestionnaires_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaires(string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetQuestionnaires_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaires(null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_GetQuestionnaire_Then_I_Get_A_QuestionnaireDto_Returned()
        {
            // arrange
            var questionnaireDto = new QuestionnaireDto();
            var surveyMock = new Mock<ISurvey>();

            _blaiseApiMock.Setup(b => b
                    .GetQuestionnaire(_questionnaireName, _serverParkName))
                .Returns(surveyMock.Object);

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDto(surveyMock.Object))
                .Returns(questionnaireDto);

            // act
            var result = _sut.GetQuestionnaire(_questionnaireName, _serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<QuestionnaireDto>(result);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_GetQuestionnaire_Then_I_Get_A_Correct_QuestionnaireDto_Returned()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var questionnaireDto = new QuestionnaireDto();
            var surveyMock = new Mock<ISurvey>();

            _blaiseApiMock.Setup(b => b
                .GetQuestionnaire(questionnaireName, serverParkName))
                .Returns(surveyMock.Object);

            _questionnaireMapperMock.Setup(m => m.MapToQuestionnaireDto(surveyMock.Object))
                .Returns(questionnaireDto);

            // act
            var result = _sut.GetQuestionnaire(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(questionnaireDto, result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaire(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaire(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaire(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaire(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_QuestionnaireExists_Then_True_Is_Returned()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b =>
                b.QuestionnaireExists(questionnaireName, serverParkName)).Returns(true);

            // act
            var result = _sut.QuestionnaireExists(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [Test]
        public void Given_A_Questionnaire_Does_Not_Exist_When_I_Call_QuestionnaireExists_Then_False_Is_Returned()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b =>
                b.QuestionnaireExists(questionnaireName, serverParkName)).Returns(false);

            // act
            var result = _sut.QuestionnaireExists(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_QuestionnaireExists_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireExists(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_QuestionnaireExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireExists(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_QuestionnaireExists_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.QuestionnaireExists(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_QuestionnaireExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.QuestionnaireExists(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_GetQuestionnaireId_Then_The_Correct_Id_Is_Returned()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";
            var questionnaireId = Guid.NewGuid();

            _blaiseApiMock.Setup(b =>
                b.GetIdOfQuestionnaire(questionnaireName, serverParkName)).Returns(questionnaireId);

            // act
            var result = _sut.GetQuestionnaireId(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(questionnaireId, result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetQuestionnaireId_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaireId(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetQuestionnaireId_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaireId(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetQuestionnaireId_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaireId(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetQuestionnaireId_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaireId(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [TestCase(QuestionnaireStatusType.Active)]
        [TestCase(QuestionnaireStatusType.Inactive)]
        [TestCase(QuestionnaireStatusType.Erroneous)]
        [TestCase(QuestionnaireStatusType.Installing)]
        [TestCase(QuestionnaireStatusType.Other)]
        public void Given_A_Questionnaire_Exists_When_I_Call_GetQuestionnaireStatus_Then_The_Correct_Status_Is_Returned(QuestionnaireStatusType surveyStatus)
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b =>
                b.GetQuestionnaireStatus(questionnaireName, serverParkName)).Returns(surveyStatus);

            // act
            var result = _sut.GetQuestionnaireStatus(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(surveyStatus, result);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_GetQuestionnaireStatus_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaireStatus(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_GetQuestionnaireStatus_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaireStatus(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_GetQuestionnaireStatus_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetQuestionnaireStatus(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_GetQuestionnaireStatus_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetQuestionnaireStatus(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_ActivateQuestionnaire_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b =>
                b.ActivateQuestionnaire(questionnaireName, serverParkName));

            // act
            _sut.ActivateQuestionnaire(questionnaireName, serverParkName);

            // assert
            _blaiseApiMock.Verify(v => v.ActivateQuestionnaire(questionnaireName, serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_ActivateQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ActivateQuestionnaire(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_ActivateQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ActivateQuestionnaire(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_ActivateQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ActivateQuestionnaire(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_ActivateQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ActivateQuestionnaire(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Exists_When_I_Call_DeactivateQuestionnaire_Then_The_Correct_Service_Is_Called()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "ServerParkA";

            _blaiseApiMock.Setup(b =>
                b.DeactivateQuestionnaire(questionnaireName, serverParkName));

            // act
            _sut.DeactivateQuestionnaire(questionnaireName, serverParkName);

            // assert
            _blaiseApiMock.Verify(v => v.DeactivateQuestionnaire(questionnaireName, serverParkName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_QuestionnaireName_When_I_Call_DeactivateQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeactivateQuestionnaire(
                string.Empty,
                _serverParkName));
            Assert.AreEqual("A value for the argument 'questionnaireName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_QuestionnaireName_When_I_Call_DeactivateQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeactivateQuestionnaire(
                null,
                _serverParkName));
            Assert.AreEqual("questionnaireName", exception.ParamName);
        }

        [Test]
        public void Given_An_Empty_ServerParkName_When_I_Call_DeactivateQuestionnaire_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.DeactivateQuestionnaire(
                _questionnaireName,
                string.Empty));
            Assert.AreEqual("A value for the argument 'serverParkName' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_ServerParkName_When_I_Call_DeactivateQuestionnaire_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.DeactivateQuestionnaire(
                _questionnaireName,
                null));
            Assert.AreEqual("serverParkName", exception.ParamName);
        }

        [Test]
        public void Given_A_Questionnaire_Has_Modes_When_I_Call_GetModes_Then_I_Get_A_List_Containing_Modes_Back()
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "LocalDevelopment";
            var modes = new List<string> { "CATI", "CAWI" };
            _blaiseApiMock.Setup(b => b.GetQuestionnaireModes(questionnaireName, serverParkName)).Returns(modes);

            // act
            var result = _sut.GetModes(questionnaireName, serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreSame(modes, result);
        }

        [TestCase("CATI")]
        [TestCase("caTi")]
        [TestCase("cawi")]
        [TestCase("CAWi")]
        public void Given_A_Mode_Exists_When_I_Call_ModeExists_With_That_Mode_Then_True_Is_Returned(string mode)
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "LocalDevelopment";
            var modes = new List<string> { "CATI", "CAWI" };
            _blaiseApiMock.Setup(b => b.GetQuestionnaireModes(questionnaireName, serverParkName)).Returns(modes);

            // act
            var result = _sut.ModeExists(questionnaireName, serverParkName, mode);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestCase("CATTTI")]
        [TestCase("caTii")]
        [TestCase("cawiw")]
        [TestCase("CAWWi")]
        public void Given_A_Mode_Does_Not_Exist_When_I_Call_ModeExists_With_That_Mode_Then_False_Is_Returned(string mode)
        {
            // arrange
            const string questionnaireName = "OPN2101A";
            const string serverParkName = "LocalDevelopment";
            var modes = new List<string> { "CATI", "CAWI" };
            _blaiseApiMock.Setup(b => b.GetQuestionnaireModes(questionnaireName, serverParkName)).Returns(modes);

            // act
            var result = _sut.ModeExists(questionnaireName, serverParkName, mode);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [Test]
        public void Given_I_Call_GetDataEntrySettings_I_Get_A_List_Of_DataEntrySettingsDtos_Back()
        {
            // arrange
            _blaiseApiMock.Setup(api => api.GetQuestionnaireDataEntrySettings(_questionnaireName, _serverParkName))
                .Returns(new List<DataEntrySettingsModel>());

            _dataEntrySettingsMapperMock.Setup(de => de.MapDataEntrySettingsDtos(new List<DataEntrySettingsModel>()))
                .Returns(new List<DataEntrySettingsDto>());

            // act
            var result = _sut.GetDataEntrySettings(_questionnaireName, _serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<DataEntrySettingsDto>>(result);
        }

        [Test]
        public void Given_I_Call_GetDataEntrySettings_I_Get_A_Valid_List_Of_DataEntrySettingsDtos_Back()
        {
            // arrange
            _blaiseApiMock.Setup(api => api.GetQuestionnaireDataEntrySettings(_questionnaireName, _serverParkName))
                .Returns(new List<DataEntrySettingsModel>());

            var dataEntrySettingsDtoList = new List<DataEntrySettingsDto>();
            _dataEntrySettingsMapperMock.Setup(de => de.MapDataEntrySettingsDtos(new List<DataEntrySettingsModel>()))
                .Returns(dataEntrySettingsDtoList);

            // act
            var result = _sut.GetDataEntrySettings(_questionnaireName, _serverParkName);

            // assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<DataEntrySettingsDto>>(result);
            Assert.AreSame(dataEntrySettingsDtoList, result);
        }
    }
}
