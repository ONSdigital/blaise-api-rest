using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;
using System.Runtime.CompilerServices;
using Blaise.Nuget.Api.Contracts.Enums;

[assembly: InternalsVisibleTo("Blaise.Api.Tests.Unit")]
namespace Blaise.Api.Core.Services
{
    public class UpdateCaseService : IUpdateCaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ICatiDataService _catiDataService;
        private readonly ILoggingService _loggingService;

        public UpdateCaseService(
            IBlaiseCaseApi blaiseApi,
            ICatiDataService catiDataService,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _catiDataService = catiDataService;
            _loggingService = loggingService;
        }

        public void UpdateExistingCaseWithOnlineData(IDataRecord onlineDataRecord, IDataRecord existingDataRecord,
            string serverParkName, string instrumentName, string primaryKey)
        {
            var nisraOutcome = _blaiseApi.GetOutcomeCode(onlineDataRecord);

            if (nisraOutcome == 0)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{primaryKey}' (HOut = 0)");

                return;
            }

            if (CaseIsCurrentlyInUseInCati(existingDataRecord))
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{primaryKey}' as the case is open in Cati");

                return;
            }

            var existingOutcome = _blaiseApi.GetOutcomeCode(existingDataRecord);

            if (existingOutcome > 542)
            {
                _loggingService.LogInfo(
                    $"Not processed: NISRA case '{primaryKey}' (Existing HOut = '{existingOutcome}'");

                return;
            }

            if (existingOutcome == 0 || nisraOutcome <= existingOutcome)
            {
                UpdateCase(onlineDataRecord, existingDataRecord, instrumentName, 
                    serverParkName, nisraOutcome);
                _loggingService.LogInfo(
                    $"processed: NISRA case '{primaryKey}' (HOut = '{nisraOutcome}' <= '{existingOutcome}') or (HOut = 0)'");

                return;
            }

            _loggingService.LogInfo(
                $"Not processed: NISRA case '{primaryKey}' (HOut = '{existingOutcome}' < '{nisraOutcome}')'");
        }

        internal void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string instrumentName,
            string serverParkName, int outcomeCode)
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

            _blaiseApi.UpdateCase(existingDataRecord, newFieldData,
                instrumentName, serverParkName);
        }

        private bool CaseIsCurrentlyInUseInCati(IDataRecord existingDataRecord)
        {
            var caseInUse = _blaiseApi.GetFieldValue(existingDataRecord, FieldNameType.CaseInUse);

            return caseInUse.IntegerValue == 1;
        }
    }
}
