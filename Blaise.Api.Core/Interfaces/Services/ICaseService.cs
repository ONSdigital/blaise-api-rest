namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Case;

    public interface ICaseService
    {
        List<string> GetCaseIds(string serverParkName, string questionnaireName);

        List<CaseStatusDto> GetCaseStatusList(string serverParkName, string questionnaireName);

        string GetPostCode(string serverParkName, string questionnaireName, string caseId);

        CaseDto GetCase(string serverParkName, string questionnaireName, string caseId);

        CaseMultikeyDto GetCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues);

        int CreateCases(List<CaseDto> fieldData, string questionnaireName, string serverParkName);

        void CreateCase(
            string serverParkName,
            string questionnaireName,
            string caseId,
            Dictionary<string, string> fieldData);

        void CreateCase(
            string serverParkName,
            string questionnaireName,
            List<string> keyNames,
            List<string> keyValues,
            Dictionary<string, string> fieldData);

        void UpdateCase(string serverParkName, string questionnaireName, string caseId, Dictionary<string, string> fieldData);

        void UpdateCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues, Dictionary<string, string> fieldData);

        void DeleteCase(string serverParkName, string questionnaireName, string caseId);

        void DeleteCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues);

        bool CaseExists(string serverParkName, string questionnaireName, string caseId);

        bool CaseExists(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues);

        List<CaseEditInformationDto> GetCaseEditInformationList(string serverParkName, string questionnaireName);
    }
}
