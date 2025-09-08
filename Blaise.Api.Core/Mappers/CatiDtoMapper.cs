namespace Blaise.Api.Core.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.Cati;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Nuget.Api.Contracts.Models;
    using StatNeth.Blaise.API.ServerManager;

    public class CatiDtoMapper : ICatiDtoMapper
    {
        private readonly IQuestionnaireStatusMapper _statusMapper;
        private readonly IQuestionnaireNodeDtoMapper _nodeDtoMapper;

        public CatiDtoMapper(
            IQuestionnaireStatusMapper statusMapper,
            IQuestionnaireNodeDtoMapper nodeDtoMapper)
        {
            _statusMapper = statusMapper;
            _nodeDtoMapper = nodeDtoMapper;
        }

        public CatiQuestionnaireDto MapToCatiQuestionnaireDto(ISurvey questionnaire, List<DateTime> surveyDays)
        {
            return new CatiQuestionnaireDto
            {
                Name = questionnaire.Name,
                Id = questionnaire.InstrumentID,
                ServerParkName = questionnaire.ServerPark,
                InstallDate = questionnaire.InstallDate,
                Status = _statusMapper.GetQuestionnaireStatus(questionnaire).ToString(),
                Nodes = _nodeDtoMapper.MapToQuestionnaireNodeDtos(questionnaire.Configuration),
                DataRecordCount = GetNumberOfDataRecords(questionnaire as ISurvey2),
                SurveyDays = surveyDays,
                Active = SurveyIsActive(surveyDays),
                ActiveToday = SurveyIsActiveToday(surveyDays),
                DeliverData = SetDeliverDataWhichIncludesADaysGraceFromLastSurveyDay(surveyDays),
            };
        }

        public DayBatchDto MapToDayBatchDto(DayBatchModel dayBatchModel)
        {
            return new DayBatchDto
            {
                DayBatchDate = dayBatchModel.DayBatchDate,
                CaseIds = dayBatchModel.CaseIds,
            };
        }

        private static bool SurveyIsActive(IReadOnlyCollection<DateTime> surveyDays)
        {
            return surveyDays.Any(s => s.Date <= DateTime.Today) &&
                   surveyDays.Any(s => s.Date >= DateTime.Today);
        }

        private static bool SurveyIsActiveToday(IEnumerable<DateTime> surveyDays)
        {
            return surveyDays.Any(s => s.Date == DateTime.Today);
        }

        private static int GetNumberOfDataRecords(ISurvey2 questionnaire)
        {
            var reportingInfo = questionnaire.GetReportingInfo();

            return reportingInfo.DataRecordCount;
        }

        private static bool SetDeliverDataWhichIncludesADaysGraceFromLastSurveyDay(IReadOnlyCollection<DateTime> surveyDays)
        {
            return SurveyIsActive(surveyDays) ||
                   (surveyDays.All(s => s.Date < DateTime.Today) &&
                   surveyDays.Any(s => s.Date == DateTime.Today.AddDays(-1)));
        }
    }
}
