using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Case
{
    public class CaseDto
    {
        public string CaseId { get; set; }

        public Dictionary<string, string> FieldData { get; set; } = new Dictionary<string, string>();
    }
}
