using Blaise.Api.Contracts.Models.Case;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.DataRecord;
using System.Collections.Generic;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface ICaseDtoMapper
    {
        List<CaseStatusDto> MapToCaseStatusDtoList(IEnumerable<CaseStatusModel> caseStatusModelList);
        CaseDto MapToCaseDto(string caseId, IDataRecord caseRecord);

        CaseMultikeyDto MapToCaseMultikeyDto(Dictionary<string, string> primaryKeyValues, IDataRecord caseRecord);
        CaseEditInformationDto MapToCaseEditInformationDto(IDataRecord caseRecord);
    }
}