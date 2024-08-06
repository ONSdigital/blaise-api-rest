using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Case;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IEditingService
    {       
        List<CaseEditInformationDto> GetCaseEditingDetailsList(string serverParkName, string questionnaireName);
    }
}