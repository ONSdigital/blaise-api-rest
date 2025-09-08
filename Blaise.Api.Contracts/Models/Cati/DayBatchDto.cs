namespace Blaise.Api.Contracts.Models.Cati
{
    using System;
    using System.Collections.Generic;

    public class DayBatchDto
    {
        public DateTime DayBatchDate { get; set; }

        public List<string> CaseIds { get; set; } = new List<string>();
    }
}
