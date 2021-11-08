using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Cati
{
    public class DayBatchDto
    {
        public DayBatchDto()
        {
            CaseIds = new List<string>();
        }

        public DateTime DayBatchDate { get; set; }

        public List<string> CaseIds { get; set; }
    }
}
