namespace Blaise.Api.Contracts.Models.Cati
{
    using System;

    public class CreateDayBatchDto
    {
        public DateTime? DayBatchDate { get; set; }

        public bool? CheckForTreatedCases { get; set; }
    }
}
