using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraCaseComparisonService
    {
        bool UpdateExistingCase(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel, 
            string instrumentName);
    }
}