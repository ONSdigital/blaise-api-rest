using System.Collections.Generic;

namespace Blaise.Api.Tests.Behaviour.Helpers.PrimaryKey
{
    public static class PrimaryKeyHelper
    {
        public static Dictionary<string, string> CreatePrimaryKeys(string caseId)
        {
            return new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
        }
    }
}
