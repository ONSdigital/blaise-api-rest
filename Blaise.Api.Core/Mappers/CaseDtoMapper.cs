using Blaise.Api.Contracts.Enums;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Mappers
{
    public class CaseDtoMapper : ICaseDtoMapper
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public CaseDtoMapper(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }

        public List<CaseStatusDto> MapToCaseStatusDtoList(IEnumerable<CaseStatusModel> caseStatusModelList)
        {
            var caseStatusDtoList = new List<CaseStatusDto>();

            foreach (var caseStatus in caseStatusModelList)
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

        public CaseDto MapToCaseDto(string caseId, IDataRecord caseRecord)
        {
            return new CaseDto
            {
                CaseId = caseId,
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public CaseMultikeyDto MapToCaseMultikeyDto(Dictionary<string, string> primaryKeyValues, IDataRecord caseRecord)
        {
            return new CaseMultikeyDto
            {
                PrimaryKeyValues = primaryKeyValues,
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public CaseEditInformationDto MapToCaseEditInformationDto(IDataRecord caseRecord)
        {
            caseRecord.ThrowExceptionIfNull("caseRecord");                

            return new CaseEditInformationDto
            {
                PrimaryKey = _blaiseCaseApi.GetFieldValue(caseRecord, "QID.Serial_Number").ValueAsText,
                Outcome = (int)_blaiseCaseApi.GetFieldValue(caseRecord, "Admin.HOut").IntegerValue,
                AssignedTo = _blaiseCaseApi.GetFieldValue(caseRecord, "QEdit.AssignedTo").ValueAsText,
                Interviewer = "", // TODO
                EditedStatus = (EditedStatusType)_blaiseCaseApi.GetFieldValue(caseRecord, "QEdit.EditedStatus").EnumerationValue,
                Organisation = (OrganisationType)_blaiseCaseApi.GetFieldValue(caseRecord, "orgID").EnumerationValue,
            };
        }
    }
}