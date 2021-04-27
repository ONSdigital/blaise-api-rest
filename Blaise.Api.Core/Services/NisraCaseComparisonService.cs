using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Services
{
    public class NisraCaseComparisonService : INisraCaseComparisonService
    {
        private readonly ILoggingService _loggingService;

        public NisraCaseComparisonService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public bool UpdateExistingCase(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel, 
            string instrumentName)
        {
            if (nisraCaseStatusModel.Outcome == 0)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (NISRA HOut = 0) for instrument '{instrumentName}'");

                return false;
            }

            if (existingCaseStatusModel.Outcome == 561 || existingCaseStatusModel.Outcome == 562)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (Existing HOut = '{existingCaseStatusModel.Outcome}' for instrument '{instrumentName}'");

                return false;
            }

            if (NisraRecordHasAlreadyBeenProcessed(nisraCaseStatusModel, existingCaseStatusModel,
                instrumentName))
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' as is has already been updated on a previous run for instrument '{instrumentName}'");

                return false;
            }
            
            if (existingCaseStatusModel.Outcome > 0 && existingCaseStatusModel.Outcome < nisraCaseStatusModel.Outcome)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (Existing HOut = '{existingCaseStatusModel.Outcome}' < '{nisraCaseStatusModel.Outcome}')  for instrument '{instrumentName}'");

                return false;
            }

            _loggingService.LogInfo(
                $"processed: NISRA case '{nisraCaseStatusModel.PrimaryKey}' (NISRA HOut = '{nisraCaseStatusModel.Outcome}' <= '{existingCaseStatusModel.Outcome}') or (Existing HOut = 0)' for instrument '{instrumentName}'");

            return true;
        }

        internal bool NisraRecordHasAlreadyBeenProcessed(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel,
            string instrumentName)
        {
            var recordHasAlreadyBeenProcessed =
                nisraCaseStatusModel.Outcome == existingCaseStatusModel.Outcome &&
                nisraCaseStatusModel.LastUpdated == existingCaseStatusModel.LastUpdated;

            _loggingService.LogInfo($"Check if NISRA case has already been processed previously '{nisraCaseStatusModel.PrimaryKey}': '{recordHasAlreadyBeenProcessed}' - " +
                                    $"(NISRA HOut = '{nisraCaseStatusModel.Outcome}' timestamp = '{nisraCaseStatusModel.LastUpdated}') " +
                                    $"(Existing HOut = '{existingCaseStatusModel.Outcome}' timestamp = '{existingCaseStatusModel.LastUpdated}')" +
                                    $" for instrument '{instrumentName}'");

            return recordHasAlreadyBeenProcessed;
        }
    }
}
