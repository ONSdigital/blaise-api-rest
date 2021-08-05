using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseSqlApi _blaiseSqlApi;

        public CaseService(IBlaiseSqlApi blaiseSqlApi)
        {
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
            return _blaiseSqlApi.GetPostCode(instrumentName, caseId);
        }
    }
}