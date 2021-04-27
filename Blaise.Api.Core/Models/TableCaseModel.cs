using Blaise.Api.Core.Enums;

namespace Blaise.Api.Core.Models
{
    public class CaseComparisonModel
    {
        public CaseComparisonModel(string primaryKey, int outcome, ModeType mode, string lastUpdated)
        {
            PrimaryKey = primaryKey;
            Outcome = outcome;
            Mode = mode;
            LastUpdated = lastUpdated;
        }

        public string PrimaryKey { get; set; }

        public int Outcome { get; set; }

        public ModeType Mode { get; set; }

        public string LastUpdated { get; set; }
    }
}
