using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;

namespace Blaise.Api.Contracts.Models.Cati
{
    public class CatiInstrumentDto : InstrumentDto
    {
        public CatiInstrumentDto()
        {
            SurveyDays = new List<DateTime>();
        }

        public List<DateTime> SurveyDays;

        public bool Active;

        public bool ActiveToday;

        public bool DeliverData;
    }
}