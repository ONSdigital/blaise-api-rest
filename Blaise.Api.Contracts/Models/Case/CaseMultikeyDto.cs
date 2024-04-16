using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Case
{
    public class CaseMultikeyDto
    {
        public CaseMultikeyDto()
        {
            PrimaryKeyValues = new Dictionary<string, string>();
            FieldData = new Dictionary<string, string>();
        }

        public Dictionary<string, string> PrimaryKeyValues { get; set; }

        public Dictionary<string, string> FieldData { get; set; }
    }
}
