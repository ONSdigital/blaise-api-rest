using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class InstrumentDtoMapper : IInstrumentDtoMapper
    {
        private readonly IInstrumentStatusMapper _statusMapper;
        private readonly IInstrumentNodeDtoMapper _nodeDtoMapper;

        public InstrumentDtoMapper(
            IInstrumentStatusMapper statusMapper, 
            IInstrumentNodeDtoMapper nodeDtoMapper)
        {
            _statusMapper = statusMapper;
            _nodeDtoMapper = nodeDtoMapper;
        }

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
                Status = _statusMapper.GetInstrumentStatus(instrument).ToString(),
                DataRecordCount = GetNumberOfDataRecords(instrument as ISurvey2),
                Nodes = _nodeDtoMapper.MapToInstrumentNodeDtos(instrument.Configuration)
            };
        }

        private static int GetNumberOfDataRecords(ISurvey2 instrument)
        {
            var reportingInfo = instrument.GetReportingInfo();

            return reportingInfo.DataRecordCount;
        }
    }
}
