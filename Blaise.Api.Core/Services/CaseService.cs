using System;
using System.Linq;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly IOnlineCaseService _onlineCaseService;
        private readonly ILoggingService _loggingService;

        public CaseService(
            IBlaiseCaseApi blaiseApi,
            IOnlineCaseService onlineCaseService,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _loggingService = loggingService;
            _onlineCaseService = onlineCaseService;
        }

        public void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName)
        {
            var telCaseTable = _onlineCaseService.GetExistingCaseComparisonModels(instrumentName, serverParkName).ToList();
            var caseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!caseRecords.EndOfSet)
            {
                var nisraRecord = caseRecords.ActiveRecord;
                var primaryKey = _blaiseApi.GetPrimaryKeyValue(nisraRecord);
                var nisraOutcome = _blaiseApi.GetOutcomeCode(nisraRecord);
                var nisraLastUpdateDate = _blaiseApi.GetFieldValue(nisraRecord, FieldNameType.LastUpdated)?.ValueAsText;

                var telEntry = telCaseTable.FirstOrDefault(t => t.PrimaryKey == primaryKey);

                if (telEntry != null && _onlineCaseService.UpdateExistingCase(nisraOutcome, telEntry.Outcome,
                    nisraLastUpdateDate, telEntry.LastUpdated,
                    primaryKey, instrumentName))
                {
                    ProcessRecord(primaryKey, nisraRecord, nisraOutcome, telEntry.Outcome, 
                        instrumentName, serverParkName);
                }

                caseRecords.MoveNext();
            }
        }

        private void ProcessRecord(string primaryKey, IDataRecord nisraRecord, 
            int nisraOutcome, int telOutcome, string instrumentName, string serverParkName)
        {
            try
            {
                var existingDataRecord = _blaiseApi.GetCase(primaryKey, instrumentName, serverParkName);

                if (_blaiseApi.CaseInUseInCati(existingDataRecord))
                {
                    _loggingService.LogInfo(
                        $"Not processed: NISRA case '{primaryKey}' as the case may be open in Cati for instrument '{instrumentName}'");

                    return;
                }

                _onlineCaseService.UpdateCase(nisraRecord, existingDataRecord, instrumentName,
                    serverParkName, nisraOutcome, telOutcome, primaryKey);
            }
            catch (Exception ex)
            {
                _loggingService.LogWarn($"Warning: There was an error updating case '{primaryKey}' - '{ex}'");
            }
        }
    }
}
