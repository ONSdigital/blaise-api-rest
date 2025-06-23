using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class QuestionnaireDtoMapper : IQuestionnaireDtoMapper
    {
        private readonly IQuestionnaireStatusMapper _statusMapper;
        private readonly IQuestionnaireNodeDtoMapper _nodeDtoMapper;

        public QuestionnaireDtoMapper(
            IQuestionnaireStatusMapper statusMapper,
            IQuestionnaireNodeDtoMapper nodeDtoMapper)
        {
            _statusMapper = statusMapper;
            _nodeDtoMapper = nodeDtoMapper;
        }

        public IEnumerable<QuestionnaireDto> MapToQuestionnaireDtos(IEnumerable<ISurvey> questionnaires)
        {
            var questionnaireDtoList = new List<QuestionnaireDto>();

            foreach (var questionnaire in questionnaires)
            {
                questionnaireDtoList.Add(MapToQuestionnaireDto(questionnaire));
            }

            return questionnaireDtoList;
        }

        public QuestionnaireDto MapToQuestionnaireDto(ISurvey questionnaire)
        {
            return new QuestionnaireDto
            {
                Name = questionnaire.Name,
                Id = questionnaire.InstrumentID,
                ServerParkName = questionnaire.ServerPark,
                InstallDate = questionnaire.InstallDate,
                FieldPeriod = GetFieldPeriod(questionnaire.Name),
                SurveyTla = GetSurveyTla(questionnaire.Name),
                Status = _statusMapper.GetQuestionnaireStatus(questionnaire).ToString(),
                DataRecordCount = GetNumberOfDataRecords(questionnaire as ISurvey2),
                BlaiseVersion = GetBlaiseVersion(questionnaire),
                Nodes = _nodeDtoMapper.MapToQuestionnaireNodeDtos(questionnaire.Configuration)
            };
        }

        public static DateTime? GetFieldPeriod(string questionnaireName)
        {
            if (questionnaireName.Length < 7)
            {
                return null;
            }

            var yearPeriod = questionnaireName.Substring(3, 2);
            var monthPeriod = questionnaireName.Substring(5, 2);

            if (int.TryParse(yearPeriod, out var year) && int.TryParse(monthPeriod, out var month))
            {
                if (month < 1 || month > 12)
                {
                    return null;
                }

                return new DateTime(2000 + year, month, 1);
            }

            return null;
        }

        public static string GetSurveyTla(string questionnaireName)
        {
            return questionnaireName.Length < 3 ? null : questionnaireName.Substring(0, 3);
        }

        private static string GetBlaiseVersion(ISurvey questionnaire)
        {
            var configuration = questionnaire.Configuration.Configurations.FirstOrDefault() as IConfiguration2;

            if (configuration == null)
            {
                return "Not Available";
            }

            return configuration.BlaiseVersion;
        }

        private static int GetNumberOfDataRecords(ISurvey2 questionnaire)
        {
            var reportingInfo = questionnaire.GetReportingInfo();

            return reportingInfo.DataRecordCount;
        }
    }
}
