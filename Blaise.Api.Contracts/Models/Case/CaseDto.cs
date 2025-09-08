namespace Blaise.Api.Contracts.Models.Case
{
    using System.Collections.Generic;

    public class CaseDto
    {
        public string CaseId { get; set; }

        public Dictionary<string, string> FieldData { get; set; } = new Dictionary<string, string>();
    }
}
