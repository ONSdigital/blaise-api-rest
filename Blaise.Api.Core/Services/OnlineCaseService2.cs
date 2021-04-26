using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class OnlineCaseService2
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ICatiDataService _catiDataService;
        private readonly ILoggingService _loggingService;

        public OnlineCaseService2(
            IBlaiseCaseApi blaiseApi,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _loggingService = loggingService;
            _catiDataService = new CatiDataService();
        }

        public bool UpdateExistingCase(int nisraOutcome, int existingOutcome, string nisraUpdatedDate, 
            string existingUpdatedDate, string primaryKey, string instrumentName)
        {
            if (nisraOutcome == 0)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{primaryKey}' (NISRA HOut = 0) for instrument '{instrumentName}'");

                return false;
            }

            if (existingOutcome == 561 || existingOutcome == 562)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{primaryKey}' (Existing HOut = '{existingOutcome}' for instrument '{instrumentName}'");

                return false;
            }

            if (NisraRecordHasAlreadyBeenProcessed(nisraOutcome, existingOutcome,
                nisraUpdatedDate, existingUpdatedDate,
                primaryKey, instrumentName))
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{primaryKey}' as is has already been updated on a previous run for instrument '{instrumentName}'");

                return false;
            }


            if (existingOutcome > 0 && existingOutcome < nisraOutcome)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{primaryKey}' (Existing HOut = '{existingOutcome}' < '{nisraOutcome}')  for instrument '{instrumentName}'");

                return false;
            }

            return true;
        }

        private bool NisraRecordHasAlreadyBeenProcessed(int nisraOutcome, int existingOutcome, string nisraUpdatedDate, 
            string existingUpdatedDate, string primaryKey, string instrumentName)
        {
            var recordHasAlreadyBeenProcessed = 
                nisraOutcome == existingOutcome && 
                nisraUpdatedDate == existingUpdatedDate;

            _loggingService.LogInfo($"Check if NISRA case has already been processed previously '{primaryKey}': '{recordHasAlreadyBeenProcessed}' - " +
                                    $"(NISRA HOut = '{nisraOutcome}' timestamp = '{nisraUpdatedDate}') " +
                                    $"(Existing HOut = '{existingOutcome}' timestamp = '{existingUpdatedDate}')" +
                                    $" for instrument '{instrumentName}'");

            return recordHasAlreadyBeenProcessed;
        }

        internal void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string instrumentName,
                   string serverParkName, int newOutcome, int existingOutcome, string primaryKey)
        {
            var fieldData = GetFieldData(newDataRecord, existingDataRecord, newOutcome);

            _blaiseApi.UpdateCase(existingDataRecord, fieldData, instrumentName, serverParkName);

            if (RecordHasBeenUpdated(primaryKey, newDataRecord, newOutcome, instrumentName, serverParkName))
            {
                _loggingService.LogInfo(
                    $"processed: NISRA case '{primaryKey}' (NISRA HOut = '{newOutcome}' <= '{existingOutcome}') or (Existing HOut = 0)' for instrument '{instrumentName}'");

                return;
            }

            _loggingService.LogWarn($"NISRA case '{primaryKey}' failed to update - potentially open in Cati at the time of the update for instrument '{instrumentName}'");
        }

        internal Dictionary<string, string> GetFieldData(IDataRecord newDataRecord, IDataRecord existingDataRecord, int outcomeCode)
        {
            var newFieldData = _blaiseApi.GetRecordDataFields(newDataRecord);
            var existingFieldData = _blaiseApi.GetRecordDataFields(existingDataRecord);

            // we need to preserve the TO CatiMana block data sp remove the fields from WEB
            _catiDataService.RemoveCatiManaBlock(newFieldData);

            // we need to preserve the TO CallHistory block data captured in Cati
            _catiDataService.RemoveCallHistoryBlock(newFieldData);

            //we need to preserve the web nudged field
            _catiDataService.RemoveWebNudgedField(newFieldData);

            // add the existing cati call data with additional items to the new field data
            _catiDataService.AddCatiManaCallItems(newFieldData, existingFieldData, outcomeCode);

            return newFieldData;
        }

        internal bool RecordHasBeenUpdated(string primaryKey, IDataRecord newDataRecord, int newOutcomeCode,
            string instrumentName, string serverParkName)
        {
            var existingRecord = _blaiseApi.GetCase(primaryKey, instrumentName, serverParkName);

            return _blaiseApi.GetOutcomeCode(existingRecord) == newOutcomeCode &&
                   _blaiseApi.GetFieldValue(existingRecord, FieldNameType.LastUpdated)?.ValueAsText == _blaiseApi.GetFieldValue(newDataRecord, FieldNameType.LastUpdated)?.ValueAsText;
        }
    }
}
