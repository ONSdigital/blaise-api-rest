namespace Blaise.Api.Contracts.Models.Case
{
    using System.Collections.Generic;

    public class CaseMultikeyDto
    {
        public Dictionary<string, string> PrimaryKeyValues { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> FieldData { get; set; } = new Dictionary<string, string>();
    }
}
