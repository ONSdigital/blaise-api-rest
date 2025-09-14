namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Case;
    using Blaise.Nuget.Api.Contracts.Models;
    using StatNeth.Blaise.API.DataRecord;

    public interface ICaseDtoMapper
    {
        List<CaseStatusDto> MapToCaseStatusDtoList(IEnumerable<CaseStatusModel> caseStatusModelList);

        CaseDto MapToCaseDto(string caseId, IDataRecord caseRecord);

        CaseMultikeyDto MapToCaseMultikeyDto(Dictionary<string, string> primaryKeyValues, IDataRecord caseRecord);

        CaseEditInformationDto MapToCaseEditInformationDto(IDataRecord caseRecord);
    }
}
