using System;

namespace Blaise.Api.Contracts.Models.Cati
{
    public class CreateDayBatchDto
    {
        public DateTime? DayBatchDate { get; set; }

        public bool? CheckForTreatedCases { get; set; }
    }
}
