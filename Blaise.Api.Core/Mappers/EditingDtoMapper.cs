using Blaise.Api.Contracts.Models.Edit;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Mappers
{
    public class EditingDtoMapper : IEditingDtoMapper
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public EditingDtoMapper(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }
        public EditingDetailsDto MapToEditingDetailsDto(IDataRecord caseRecord)
        {
            caseRecord.ThrowExceptionIfNull("caseRecord");                

            var editingDetailsDto = new EditingDetailsDto
            {
                PrimaryKey = _blaiseCaseApi.GetFieldValue(caseRecord, "QID.Serial_Number").ValueAsText,
                Outcome = (int)_blaiseCaseApi.GetFieldValue(caseRecord, "Admin.HOut").IntegerValue,
                AssignedTo = _blaiseCaseApi.GetFieldValue(caseRecord, "QEdit.AssignedTo").ValueAsText,
                EditedStatus = (int)_blaiseCaseApi.GetFieldValue(caseRecord, "QEdit.EditedStatus").IntegerValue,
                // TODO
                Interviewer = "",
            };
            return editingDetailsDto;
        }
    }
}