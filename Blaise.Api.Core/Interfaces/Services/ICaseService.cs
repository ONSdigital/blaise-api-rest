using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Case;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICaseService
    {
        List<string> GetCaseIds(string serverParkName, string questionnaireName);

        List<CaseStatusDto> GetCaseStatusList(string serverParkName, string questionnaireName);

        string GetPostCode(string serverParkName, string questionnaireName, string caseId);

        CaseDto GetCase(string serverParkName, string questionnaireName, string caseId);

        void CreateCase(string serverParkName, string questionnaireName, string caseId,
            Dictionary<string, string> fieldData);

        void UpdateCase(string serverParkName, string questionnaireName, string caseId, Dictionary<string, string> fieldData);
        
        void DeleteCase(string serverParkName, string questionnaireName, string caseId);
        bool CaseExists(string serverParkName, string questionnaireName, string caseId);
    }
}