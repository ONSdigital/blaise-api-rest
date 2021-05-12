using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class CatiInstrumentDtoMapper : ICatiInstrumentDtoMapper
    {
        private readonly IInstrumentStatusMapper _statusMapper;
        private readonly IInstrumentNodeDtoMapper _nodeDtoMapper;

        public CatiInstrumentDtoMapper(
            IInstrumentStatusMapper statusMapper, 
            IInstrumentNodeDtoMapper nodeDtoMapper)
        {
            _statusMapper = statusMapper;
            _nodeDtoMapper = nodeDtoMapper;
        }

        public CatiInstrumentDto MapToCatiInstrumentDto(ISurvey instrument, List<DateTime> surveyDays,
            DateTime? liveDate)
        {
            return new CatiInstrumentDto
            {
                Name = instrument.Name,
                ServerParkName = instrument.ServerPark,
                InstallDate = instrument.InstallDate,
                Status = _statusMapper.GetInstrumentStatus(instrument).ToString(),
                Nodes = _nodeDtoMapper.MapToInstrumentNodeDtos(instrument.Configuration),
                DataRecordCount = GetNumberOfDataRecords(instrument as ISurvey2),
                SurveyDays = surveyDays,
                Active = SurveyIsActive(surveyDays),
                ActiveToday = SurveyIsActiveToday(surveyDays),
                DeliverData = SetDeliverDataWhichIncludesADaysGraceFromLastSurveyDay(surveyDays)
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

        private static int GetNumberOfDataRecords(ISurvey2 instrument)
        {
            var reportingInfo = instrument.GetReportingInfo();

            return reportingInfo.DataRecordCount;
        }

        private static bool SetDeliverDataWhichIncludesADaysGraceFromLastSurveyDay(IReadOnlyCollection<DateTime> surveyDays)
        {
            return SurveyIsActive(surveyDays) || 
                   surveyDays.All(s => s.Date < DateTime.Today) &&
                   surveyDays.Any(s => s.Date == DateTime.Today.AddDays(-1));
        }
    }
}
