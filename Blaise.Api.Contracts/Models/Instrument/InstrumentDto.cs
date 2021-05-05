using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Instrument
{
    public class InstrumentDto
    {
        public InstrumentDto()
        {
            Nodes = new List<InstrumentNodeDto>();
        }

        public string Name { get; set; }
        public string ServerParkName { get; set; }
        public DateTime InstallDate { get; set; }
        public string Status { get; set; }
        public int DataRecordCount { get; set; }
        public bool HasData => DataRecordCount > 0;

        public IEnumerable<InstrumentNodeDto> Nodes { get; set; }
    }
}
