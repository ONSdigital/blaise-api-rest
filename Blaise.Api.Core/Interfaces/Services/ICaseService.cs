using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Case;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICaseService
    {
        List<string> GetCaseIds(string instrumentName);
        IEnumerable<CaseIdentifierDto> GetCaseIdentifiers(string instrumentName);
        string GetPostCode(string serverParkName, string instrumentName, string caseId);
    }
}