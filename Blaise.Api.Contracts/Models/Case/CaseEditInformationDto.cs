﻿namespace Blaise.Api.Contracts.Models.Case
{
    public class CaseEditInformationDto
    {
        public string PrimaryKey { get; set; }

        public int Outcome { get; set; }

        public string AssignedTo { get; set; }

        public string Interviewer { get; set; }

        public int EditedStatus { get; set; }

        public int Organisation { get; set; }
    }
}
