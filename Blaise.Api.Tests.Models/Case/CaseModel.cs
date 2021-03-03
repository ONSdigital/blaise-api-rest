using System;
using Blaise.Api.Tests.Models.Enums;

namespace Blaise.Api.Tests.Models.Case
{
    public class CaseModel
    {
        public CaseModel(string primaryKey, string outcome, ModeType mode)
        {
            PrimaryKey = primaryKey;
            Outcome = outcome;
            Mode = mode;
            LastUpdated = DateTime.Now;
        }

        public CaseModel(string primaryKey, string outcome, ModeType mode, DateTime lastUpdated)
        {
            PrimaryKey = primaryKey;
            Outcome = outcome;
            Mode = mode;
        }

        public string PrimaryKey { get; set; }

        public string Outcome { get; set; }

        public ModeType Mode { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
