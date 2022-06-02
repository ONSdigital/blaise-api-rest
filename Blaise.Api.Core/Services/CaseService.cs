using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public CaseService(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }

        public List<string> GetCaseIds(string serverParkName, string questionnaireName)
        {
            var caseStatusList = _blaiseCaseApi.GetCaseStatusModelList(questionnaireName, serverParkName);

            return caseStatusList.Select(caseStatus => caseStatus.PrimaryKey).ToList();
        }

        public List<CaseStatusDto> GetCaseStatusList(string serverParkName, string questionnaireName)
        {
            var caseStatusList = _blaiseCaseApi.GetCaseStatusModelList(questionnaireName, serverParkName);
            var caseStatusDtoList = new List<CaseStatusDto>();

            foreach (var caseStatus in caseStatusList)
            {
                caseStatusDtoList.Add(

                    new CaseStatusDto
                    {
                        PrimaryKey = caseStatus.PrimaryKey,
                        Outcome = caseStatus.Outcome
                    });

            }

            return caseStatusDtoList;
        }

        public string GetPostCode(string serverParkName, string questionnaireName, string caseId)
        {
            var caseRecord = _blaiseCaseApi.GetCase(caseId, questionnaireName, serverParkName);

            return _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.PostCode).ValueAsText;
        }

        public CaseDto GetCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var caseRecord = _blaiseCaseApi.GetCase(caseId, questionnaireName, serverParkName);

            return new CaseDto
            {
                CaseId = _blaiseCaseApi.GetPrimaryKeyValue(caseRecord),
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public void CreateCase(string serverParkName, string questionnaireName, string caseId,
            Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            _blaiseCaseApi.CreateCase(caseId, fieldData, questionnaireName, serverParkName);
        }

        public void UpdateCase(string serverParkName, string questionnaireName, string caseId, Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            _blaiseCaseApi.UpdateCase(caseId, fieldData, questionnaireName, serverParkName);
        }

        public void DeleteCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            _blaiseCaseApi.RemoveCase(caseId, questionnaireName, serverParkName);
        }
    }
}