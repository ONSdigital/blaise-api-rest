using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Case;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICaseService
    {
        List<string> GetCaseIds(string serverParkName, string instrumentName);

        string GetPostCode(string serverParkName, string instrumentName, string caseId);

        CaseDto GetCase(string serverParkName, string instrumentName, string caseId);

        void CreateCase(string serverParkName, string instrumentName, string caseId,
            Dictionary<string, string> fieldData);

        void UpdateCase(string serverParkName, string instrumentName, string caseId, Dictionary<string, string> fieldData);
    }
}