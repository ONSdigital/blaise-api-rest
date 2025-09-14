namespace Blaise.Api.Tests.Unit.Helpers
{
    using System.Collections.Generic;

    public static class PrimaryKeyHelper
    {
        public static Dictionary<string, string> CreatePrimaryKeys(string caseId)
        {
            return new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
        }
    }
}
