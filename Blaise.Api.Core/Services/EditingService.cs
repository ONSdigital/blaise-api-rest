using Blaise.Api.Contracts.Models.Edit;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Mappers;

namespace Blaise.Api.Core.Services
{
    public class EditingService : IEditingService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseSqlApi _blaiseSqlApi;
        private readonly IEditingDtoMapper _editingDtoMapper;

        public EditingService(IBlaiseCaseApi blaiseCaseApi, IBlaiseSqlApi blaiseCSqlApi, IEditingDtoMapper editingDtoMapper)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseSqlApi = blaiseCSqlApi;
            _editingDtoMapper = editingDtoMapper;
        }

        public List<EditingDetailsDto> GetCaseEditingDetailsList(string serverParkName, string questionnaireName)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            
            var caseEditingDetailsList = new List<EditingDetailsDto>();
            var caseIds = _blaiseSqlApi.GetCaseIds(questionnaireName);

            foreach ( var caseId in caseIds)
            {
                var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
                var caseRecord = _blaiseCaseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);
                
                caseEditingDetailsList.Add(_editingDtoMapper.MapToEditingDetailsDto(caseRecord));
            }
            
            return caseEditingDetailsList;
        }
    }
}