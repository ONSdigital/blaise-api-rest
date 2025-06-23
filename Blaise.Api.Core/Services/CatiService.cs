using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Exceptions;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;

// ReSharper disable PossibleInvalidOperationException
namespace Blaise.Api.Core.Services
{
    public class CatiService : ICatiService
    {
        private readonly IBlaiseCatiApi _blaiseCatiApi;
        private readonly IBlaiseServerParkApi _blaiseServerParkApi;
        private readonly ICatiDtoMapper _mapper;

        public CatiService(
            IBlaiseCatiApi blaiseApi,
            IBlaiseServerParkApi blaiseServerParkApi,
            ICatiDtoMapper mapper)
        {
            _blaiseCatiApi = blaiseApi;
            _blaiseServerParkApi = blaiseServerParkApi;
            _mapper = mapper;
        }

        public List<CatiQuestionnaireDto> GetCatiQuestionnaires()
        {
            var catiQuestionnaires = new List<CatiQuestionnaireDto>();
            var serverParks = _blaiseServerParkApi.GetNamesOfServerParks();

            foreach (var serverPark in serverParks)
            {
                var questionnaires = _blaiseCatiApi.GetInstalledQuestionnaires(serverPark);
                catiQuestionnaires.AddRange(GetCatiQuestionnaires(questionnaires));
            }

            return catiQuestionnaires;
        }

        public List<CatiQuestionnaireDto> GetCatiQuestionnaires(string serverParkName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var questionnaires = _blaiseCatiApi.GetInstalledQuestionnaires(serverParkName);

            return GetCatiQuestionnaires(questionnaires);
        }

        public CatiQuestionnaireDto GetCatiQuestionnaire(string serverParkName, string questionnaireName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var questionnaire = _blaiseCatiApi.GetInstalledQuestionnaire(questionnaireName, serverParkName);

            return GetCatiQuestionnaireDto(questionnaire);
        }

        public DayBatchDto CreateDayBatch(string questionnaireName, string serverParkName, CreateDayBatchDto createDayBatchDto)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            createDayBatchDto.ThrowExceptionIfNull("createDayBatchDto");
            createDayBatchDto.DayBatchDate.ThrowExceptionIfNull("createDayBatchDto.DayBatchDate");
            createDayBatchDto.CheckForTreatedCases.ThrowExceptionIfNull("createDayBatchDto.CheckForTreatedCases");

            var dayBatchModel = _blaiseCatiApi.CreateDayBatch(questionnaireName, serverParkName,
                (DateTime)createDayBatchDto.DayBatchDate, (bool)createDayBatchDto.CheckForTreatedCases);

            return _mapper.MapToDayBatchDto(dayBatchModel);
        }

        public DayBatchDto GetDayBatch(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var dayBatchModel = _blaiseCatiApi.GetDayBatch(questionnaireName, serverParkName);

            if (dayBatchModel == null)
            {
                throw new DataNotFoundException("No daybatch found");
            }

            return _mapper.MapToDayBatchDto(dayBatchModel);
        }

        public bool QuestionnaireHasADayBatchForToday(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var dayBatchModel = _blaiseCatiApi.GetDayBatch(questionnaireName, serverParkName);

            return dayBatchModel?.DayBatchDate.Date == DateTime.Today;
        }

        public void AddCasesToDayBatch(string questionnaireName, string serverParkName, List<string> caseIds)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            caseIds.ThrowExceptionIfNullOrEmpty("caseIds");

            foreach (var caseId in caseIds)
            {
                _blaiseCatiApi.AddToDayBatch(questionnaireName, serverParkName, caseId);
            }
        }

        public List<DateTime> GetSurveyDays(string questionnaireName, string serverParkName)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            var surveyDays = _blaiseCatiApi.GetSurveyDays(questionnaireName, serverParkName);

            return surveyDays;
        }

        public List<DateTime> AddSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            surveyDays.ThrowExceptionIfNullOrEmpty("surveyDays");

            _blaiseCatiApi.SetSurveyDays(questionnaireName, serverParkName, surveyDays);

            return GetSurveyDays(questionnaireName, serverParkName);
        }

        public void RemoveSurveyDays(string questionnaireName, string serverParkName, List<DateTime> surveyDays)
        {
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            surveyDays.ThrowExceptionIfNullOrEmpty("surveyDays");

            _blaiseCatiApi.RemoveSurveyDays(questionnaireName, serverParkName, surveyDays);
        }

        private List<CatiQuestionnaireDto> GetCatiQuestionnaires(IEnumerable<ISurvey> questionnaires)
        {
            var catiQuestionnaires = new List<CatiQuestionnaireDto>();

            foreach (var questionnaire in questionnaires)
            {
                catiQuestionnaires.Add(GetCatiQuestionnaireDto(questionnaire));
            }

            return catiQuestionnaires;
        }

        private CatiQuestionnaireDto GetCatiQuestionnaireDto(ISurvey questionnaire)
        {
            var surveyDays = _blaiseCatiApi.GetSurveyDays(questionnaire.Name, questionnaire.ServerPark);

            return _mapper.MapToCatiQuestionnaireDto(questionnaire, surveyDays);
        }
    }
}
