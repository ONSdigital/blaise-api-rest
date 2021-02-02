using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class InstrumentDtoMapper : IInstrumentDtoMapper
    {
        public IEnumerable<InstrumentDto> MapToInstrumentDtos(IEnumerable<ISurvey> instruments)
        {
            var instrumentDtoList = new List<InstrumentDto>();

            foreach (var instrument in instruments)
            {
                instrumentDtoList.Add(MapToInstrumentDto(instrument));
            }

            return instrumentDtoList;
        }

        public InstrumentDto MapToInstrumentDto(ISurvey instrument)
        {
            return new InstrumentDto
            {
                Name = instrument.Name,
                ServerParkName = instrument.ServerPark,
                InstallDate = instrument.InstallDate,
                Status = instrument.Status,
                DataRecordCount = GetNumberOfDataRecords(instrument as ISurvey2)
            };
        }

        public CatiInstrumentDto MapToCatiInstrumentDto(ISurvey instrument, List<DateTime> surveyDays)
        {
            var catiInstrument = new CatiInstrumentDto
            {
                Name = instrument.Name,
                ServerParkName = instrument.ServerPark,
                InstallDate = instrument.InstallDate,
                Status = instrument.Status,
                DataRecordCount = GetNumberOfDataRecords(instrument as ISurvey2),
                SurveyDays = surveyDays,
                Active = surveyDays.Any(s => s.Date <= DateTime.Today) &&
                         surveyDays.Any(s => s.Date >= DateTime.Today),
                ActiveToday = surveyDays.Any(s => s.Date == DateTime.Today)

            };

            catiInstrument.DeliverData = SetDeliverDataWhichIncludesADayGraceFromLastSurveyDay(catiInstrument);
            
            return catiInstrument;
        }

        private static int GetNumberOfDataRecords(ISurvey2 instrument)
        {
            var reportingInfo = instrument.GetReportingInfo();

            return reportingInfo.DataRecordCount;
        }

        private static bool SetDeliverDataWhichIncludesADayGraceFromLastSurveyDay(CatiInstrumentDto catiInstrument)
        {
            return catiInstrument.Active || 
                   catiInstrument.SurveyDays.All(s => s.Date < DateTime.Today) &&
                   catiInstrument.SurveyDays.Any(s => s.Date == DateTime.Today.AddDays(-1));
        }
    }
}
