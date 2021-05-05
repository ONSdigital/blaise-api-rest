using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraCaseComparisonService
    {
        bool CaseNeedsToBeUpdated(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel, 
            string instrumentName);
    }
}