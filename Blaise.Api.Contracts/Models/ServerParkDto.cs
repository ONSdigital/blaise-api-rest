using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models
{
    public class ServerParkDto
    {
        public ServerParkDto()
        {
            Instruments = new List<InstrumentDto>();
        }

        public string Name { get; set; }

        public IEnumerable<InstrumentDto> Instruments { get; set; }
    }
}
