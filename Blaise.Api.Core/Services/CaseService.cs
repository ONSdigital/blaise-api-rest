using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseSqlApi _blaiseSqlApi;

        public CaseService(
            IBlaiseCaseApi blaiseCaseApi, 
            IBlaiseSqlApi blaiseSqlApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseSqlApi = blaiseSqlApi;
        }

        public List<string> GetCaseIds(string instrumentName)
        {
            return _blaiseSqlApi.GetCaseIds(instrumentName).ToList();
        }

        public IEnumerable<CaseIdentifierDto> GetCaseIdentifiers(string instrumentName)
        {
            var caseIdentifierDtos = new List<CaseIdentifierDto>();
            var caseIdentifierModels = _blaiseSqlApi.GetCaseIdentifiers(instrumentName);

            foreach (var caseIdentifierModel in caseIdentifierModels)
            {
                caseIdentifierDtos.Add(new CaseIdentifierDto
                {
                    Caseid = caseIdentifierModel.PrimaryKey,
                    PostCode = caseIdentifierModel.PostCode
                });
            }

            return caseIdentifierDtos;
        }

        public string GetPostCode(string serverParkName, string instrumentName, string caseId)
        {
            var caseRecord = _blaiseCaseApi.GetCase(caseId, instrumentName, serverParkName);

            return _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.PostCode).ValueAsText;
        }
    }
}