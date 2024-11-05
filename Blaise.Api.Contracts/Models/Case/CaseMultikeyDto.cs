using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Case
{
    public class CaseMultikeyDto
    {
        public Dictionary<string, string> PrimaryKeyValues { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> FieldData { get; set; } = new Dictionary<string, string>();
    }
}
