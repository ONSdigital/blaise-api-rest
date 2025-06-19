using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class NisraCaseUpdateService : INisraCaseUpdateService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ICatiDataBlockService _catiDataService;
        private readonly ILoggingService _loggingService;

        public NisraCaseUpdateService(
            IBlaiseCaseApi blaiseApi,
            ICatiDataBlockService catiDataService,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _loggingService = loggingService;
            _catiDataService = catiDataService;
        }

        public void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string questionnaireName, string serverParkName)
        {
            var primaryKeyValues = _blaiseApi.GetPrimaryKeyValues(newDataRecord);
            var primaryKey = primaryKeyValues["QID.Serial_Number"];

            try
            {
                if (_blaiseApi.CaseInUseInCati(existingDataRecord))
                {
                    _loggingService.LogInfo(
                        $"Not processed: NISRA case '{primaryKey}' as the case may be open in Cati for questionnaire '{questionnaireName}'");

                    return;
                }

                var fieldData = GetFieldData(newDataRecord, existingDataRecord);

                _blaiseApi.UpdateCase(existingDataRecord, fieldData, questionnaireName, serverParkName);

                if (RecordHasBeenUpdated(primaryKey, newDataRecord, questionnaireName, serverParkName))
                {
                    _loggingService.LogInfo(
                        $"NISRA case '{primaryKey}' was successfully updated for questionnaire '{questionnaireName}'");

                    return;
                }

                _loggingService.LogWarn($"NISRA case '{primaryKey}' failed to update - potentially open in Cati at the time of the update for questionnaire '{questionnaireName}'");
            }
            catch (Exception ex)
            {
                _loggingService.LogWarn($"Warning: There was an error updating case '{primaryKey}' - '{ex}'");
            }
        }

        internal Dictionary<string, string> GetFieldData(IDataRecord newDataRecord, IDataRecord existingDataRecord)
        {
            var newFieldData = _blaiseApi.GetRecordDataFields(newDataRecord);
            var existingFieldData = _blaiseApi.GetRecordDataFields(existingDataRecord);

            // we need to preserve the TO CatiMana block data sp remove the fields from WEB
            _catiDataService.RemoveCatiManaBlock(newFieldData);

            // we need to preserve the TO CallHistory block data captured in Cati
            _catiDataService.RemoveCallHistoryBlock(newFieldData);

            // we need to preserve the web nudged field
            _catiDataService.RemoveWebNudgedField(newFieldData);

            // add the existing cati call data with additional items to the new field data
            var outcomeCode = _blaiseApi.GetOutcomeCode(newDataRecord);
            _catiDataService.AddCatiManaCallItems(newFieldData, existingFieldData, outcomeCode);

            return newFieldData;
        }

        internal bool RecordHasBeenUpdated(string primaryKey, IDataRecord newDataRecord,
            string questionnaireName, string serverParkName)
        {
            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", primaryKey } };
            var existingRecord = _blaiseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

            return _blaiseApi.GetLastUpdatedAsString(existingRecord) == _blaiseApi.GetLastUpdatedAsString(newDataRecord);
        }
    }
}
