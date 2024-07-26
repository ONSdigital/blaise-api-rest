using Blaise.Api.Contracts.Models.Edit;
using System.Collections.Generic;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IEditingService
    {       
        List<EditingDetailsDto> GetCaseEditingDetailsList(string serverParkName, string questionnaireName);
    }
}