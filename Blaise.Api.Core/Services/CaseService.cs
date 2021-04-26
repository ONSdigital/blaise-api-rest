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
        private readonly ILoggingService _loggingService;
        private readonly OnlineCaseService2 _caseService2;

        public CaseService(
            IBlaiseCaseApi blaiseApi,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _loggingService = loggingService;
            _caseService2 = new OnlineCaseService2(_blaiseApi, _loggingService);
        }

        public void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName)
        {
            var caseTableService = new CaseTableService(_blaiseApi);
            var telCaseTable = caseTableService.GetTelTable(instrumentName, serverParkName).ToList();
            var caseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!caseRecords.EndOfSet)
            {
                var nisraRecord = caseRecords.ActiveRecord;
                var primaryKey = _blaiseApi.GetPrimaryKeyValue(nisraRecord);
                var nisraOutcome = _blaiseApi.GetOutcomeCode(nisraRecord);
                var nisraLastUpdateDate = _blaiseApi.GetFieldValue(nisraRecord, FieldNameType.LastUpdated)?.ValueAsText;

                var telEntry = telCaseTable.First(t => t.PrimaryKey == primaryKey);
                if (_caseService2.UpdateExistingCase(nisraOutcome, telEntry.Outcome,
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
                var existingRecord = _blaiseApi.GetCase(primaryKey, instrumentName, serverParkName);

                if (_blaiseApi.CaseInUseInCati(existingRecord))
                {
                    _loggingService.LogInfo(
                        $"Not processed: NISRA case '{primaryKey}' as the case may be open in Cati for instrument '{instrumentName}'");

                    return;
                }

                _caseService2.UpdateCase(nisraRecord, existingRecord, instrumentName,
                    serverParkName, nisraOutcome, telOutcome, primaryKey);
            }
            catch (Exception ex)
            {
                _loggingService.LogWarn($"Warning: There was an error updating case '{primaryKey}' - '{ex}'");
            }
        }
    }
}
