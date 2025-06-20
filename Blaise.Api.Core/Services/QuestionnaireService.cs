using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;
        private readonly IQuestionnaireDtoMapper _questionnaireDtoMapper;
        private readonly IDataEntrySettingsDtoMapper _dataEntrySettingsDtoMapper;

        public QuestionnaireService(
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi,
            IQuestionnaireDtoMapper questionnaireDtoMapper, 
            IDataEntrySettingsDtoMapper dataEntryDtoMapper)
        {
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
            _questionnaireDtoMapper = questionnaireDtoMapper;
            _dataEntrySettingsDtoMapper = dataEntryDtoMapper;
        }

        public IEnumerable<QuestionnaireDto> GetAllQuestionnaires()
        {
            var questionnaires = _blaiseQuestionnaireApi.GetQuestionnairesAcrossServerParks();
            return _questionnaireDtoMapper.MapToQuestionnaireDtos(questionnaires);
        }

        public IEnumerable<QuestionnaireDto> GetQuestionnaires(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var questionnaires = _blaiseQuestionnaireApi.GetQuestionnaires(serverParkName);
            return _questionnaireDtoMapper.MapToQuestionnaireDtos(questionnaires);
        }

        public QuestionnaireDto GetQuestionnaire(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var questionnaire = _blaiseQuestionnaireApi.GetQuestionnaire(questionnaireName, serverParkName);
            return _questionnaireDtoMapper.MapToQuestionnaireDto(questionnaire);
        }

        public bool QuestionnaireExists(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseQuestionnaireApi.QuestionnaireExists(questionnaireName, serverParkName);
        }

        public Guid GetQuestionnaireId(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseQuestionnaireApi.GetIdOfQuestionnaire(questionnaireName, serverParkName);
        }

        public QuestionnaireStatusType GetQuestionnaireStatus(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            return _blaiseQuestionnaireApi.GetQuestionnaireStatus(questionnaireName, serverParkName);
        }

        public void ActivateQuestionnaire(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseQuestionnaireApi.ActivateQuestionnaire(questionnaireName, serverParkName);
        }

        public void DeactivateQuestionnaire(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseQuestionnaireApi.DeactivateQuestionnaire(questionnaireName, serverParkName);
        }

        public IEnumerable<string> GetModes(string questionnaireName, string serverParkName)
        {
            return _blaiseQuestionnaireApi.GetQuestionnaireModes(questionnaireName, serverParkName);
        }

        public bool ModeExists(string questionnaireName, string serverParkName, string mode)
        {
            var modeList = _blaiseQuestionnaireApi.GetQuestionnaireModes(questionnaireName, serverParkName);

            return modeList.Any(m => m.Equals(mode, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<DataEntrySettingsDto> GetDataEntrySettings(string questionnaireName, string serverParkName)
        {
            var dataEntrySettingsModels = _blaiseQuestionnaireApi.GetQuestionnaireDataEntrySettings(questionnaireName, serverParkName);

            return _dataEntrySettingsDtoMapper.MapDataEntrySettingsDtos(dataEntrySettingsModels);
        }
    }
}
