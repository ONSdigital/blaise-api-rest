
using Blaise.Api.Contracts.Models.Edit;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IEditingDtoMapper
    {
        EditingDetailsDto MapToEditingDetailsDto(IDataRecord caseRecord);
    }
}