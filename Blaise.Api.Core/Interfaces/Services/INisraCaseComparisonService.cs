namespace Blaise.Api.Core.Interfaces.Services
{
    using Blaise.Nuget.Api.Contracts.Models;

    public interface INisraCaseComparisonService
    {
        bool CaseNeedsToBeUpdated(
            CaseStatusModel nisraCaseStatusModel,
            CaseStatusModel existingCaseStatusModel,
            string questionnaireName);
    }
}
