using System.Collections.Generic;
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

        public List<string> GetCaseIds(string serverParkName, string instrumentName)
        {
            var caseIds = new List<string>();

            var cases = _blaiseCaseApi.GetCases(instrumentName, serverParkName);

            while (!cases.EndOfSet)
            {
                var primaryKey = _blaiseCaseApi.GetPrimaryKeyValue(cases.ActiveRecord);

                caseIds.Add(primaryKey);

                cases.MoveNext();
            }

            return caseIds;
        }

        public string GetPostCode(string serverParkName, string instrumentName, string caseId)
        {
            var caseRecord = _blaiseCaseApi.GetCase(caseId, instrumentName, serverParkName);

            return _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.PostCode).ValueAsText;
        }

        public CaseDto GetCase(string serverParkName, string instrumentName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var caseRecord = _blaiseCaseApi.GetCase(caseId, instrumentName, serverParkName);

            return new CaseDto
            {
                CaseId = _blaiseCaseApi.GetPrimaryKeyValue(caseRecord),
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public void CreateCase(string serverParkName, string instrumentName, string caseId, 
            Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            _blaiseCaseApi.CreateCase(caseId, fieldData, instrumentName, serverParkName);
        }
    }
}