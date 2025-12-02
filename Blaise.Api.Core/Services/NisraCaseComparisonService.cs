namespace Blaise.Api.Core.Services
{
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Nuget.Api.Contracts.Models;

    public class NisraCaseComparisonService : INisraCaseComparisonService
    {
        private readonly ILoggingService _loggingService;

        public NisraCaseComparisonService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public bool CaseNeedsToBeUpdated(
            CaseStatusModel nisraCaseStatusModel,
            CaseStatusModel existingCaseStatusModel,
            string questionnaireName)
        {
            if (nisraCaseStatusModel.Outcome == 0)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (NISRA HOut = 0) for questionnaire '{questionnaireName}'");

                return false;
            }

            if (existingCaseStatusModel.Outcome == 561 || existingCaseStatusModel.Outcome == 562)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (Existing HOut = '{existingCaseStatusModel.Outcome}' for questionnaire '{questionnaireName}'");

                return false;
            }

            if (NisraRecordHasAlreadyBeenProcessed(nisraCaseStatusModel, existingCaseStatusModel, questionnaireName))
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' as is has already been updated on a previous run for questionnaire '{questionnaireName}'");

                return false;
            }

            if (nisraCaseStatusModel.Outcome == 580 && existingCaseStatusModel.Outcome != 0 && existingCaseStatusModel.Outcome <= 210)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (Existing HOut = '{existingCaseStatusModel.Outcome}' < '{nisraCaseStatusModel.Outcome}') for questionnaire '{questionnaireName}'");

                return false;
            }

            if (nisraCaseStatusModel.Outcome != 580 && existingCaseStatusModel.Outcome > 0 && existingCaseStatusModel.Outcome < nisraCaseStatusModel.Outcome)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (Existing HOut = '{existingCaseStatusModel.Outcome}' < '{nisraCaseStatusModel.Outcome}') for questionnaire '{questionnaireName}'");

                return false;
            }

            _loggingService.LogInfo(
                $"processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (NISRA HOut = '{nisraCaseStatusModel.Outcome}' <= '{existingCaseStatusModel.Outcome}') or (Existing HOut = 0)' for questionnaire '{questionnaireName}'");

            return true;
        }

        internal bool NisraRecordHasAlreadyBeenProcessed(
            CaseStatusModel nisraCaseStatusModel,
            CaseStatusModel existingCaseStatusModel,
            string questionnaireName)
        {
            var recordHasAlreadyBeenProcessed =
                nisraCaseStatusModel.Outcome == existingCaseStatusModel.Outcome &&
                nisraCaseStatusModel.LastUpdated == existingCaseStatusModel.LastUpdated;

            _loggingService.LogInfo($"Check if NISRA case has already been processed previously '{nisraCaseStatusModel.PrimaryKey}': '{recordHasAlreadyBeenProcessed}' - " +
                                    $"(NISRA HOut = '{nisraCaseStatusModel.Outcome}' timestamp = '{nisraCaseStatusModel.LastUpdated}') " +
                                    $"(Existing HOut = '{existingCaseStatusModel.Outcome}' timestamp = '{existingCaseStatusModel.LastUpdated}')" +
                                    $" for questionnaire '{questionnaireName}'");

            return recordHasAlreadyBeenProcessed;
        }
    }
}
