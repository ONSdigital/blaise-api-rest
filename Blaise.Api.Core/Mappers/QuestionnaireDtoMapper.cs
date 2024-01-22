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
                Status = _statusMapper.GetQuestionnaireStatus(questionnaire).ToString(),
                DataRecordCount = GetNumberOfDataRecords(questionnaire as ISurvey2),
                BlaiseVersion = GetBlaiseVersion(questionnaire),
                Nodes = _nodeDtoMapper.MapToQuestionnaireNodeDtos(questionnaire.Configuration)
            };
        }

        private string GetBlaiseVersion(ISurvey questionnaire)
        {
            IConfiguration2 configuration = questionnaire.Configuration.Configurations.FirstOrDefault() as IConfiguration2;

            if (configuration == null)
            {
                return "Not Available";
            }

            return configuration.BlaiseVersion;
        }

        private static int GetNumberOfDataRecords(ISurvey2 questionnaire)
        {
            return 0;
            //var reportingInfo = questionnaire.GetReportingInfo();

            //return reportingInfo.DataRecordCount;
        }
    }
}
