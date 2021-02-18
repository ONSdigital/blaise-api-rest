using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Blaise.Api.Tests.Unit")]
namespace Blaise.Api.Core.Services
{
    public class UpdateCaseService : IUpdateCaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ILoggingService _loggingService;

        public UpdateCaseService(
            IBlaiseCaseApi blaiseApi,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _loggingService = loggingService;
        }

        public void UpdateExistingCaseWithOnlineData(IDataRecord onlineDataRecord, IDataRecord existingDataRecord, string serverParkName, string instrumentName, string serialNumber)
        {
            var nisraOutcome = _blaiseApi.GetOutcomeCode(onlineDataRecord);

            if (nisraOutcome == 0)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{serialNumber}' (HOut = 0)");

                return;
            }

            var existingOutcome = _blaiseApi.GetOutcomeCode(existingDataRecord);

            if (existingOutcome > 542)
            {
                _loggingService.LogInfo($"Not processed: NISRA case '{serialNumber}' (Existing HOut = '{existingOutcome}'");

                return;
            }

            if (existingOutcome == 0 || nisraOutcome <= existingOutcome)
            {
                UpdateCase(onlineDataRecord, existingDataRecord, instrumentName, serverParkName);
                _loggingService.LogInfo($"processed: NISRA case '{serialNumber}' (HOut = '{nisraOutcome}' <= '{existingOutcome}') or (HOut = 0)'");

                return;
            }

            _loggingService.LogInfo($"Not processed: NISRA case '{serialNumber}' (HOut = '{existingOutcome}' < '{nisraOutcome}')'");
        }

        internal void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string instrumentName,
            string serverParkName)
        {
            var fieldData = _blaiseApi.GetRecordDataFields(newDataRecord);

            // we need to preserve the TO CatiMana block data sp remove the fields from WEB
            RemovedCataManaBlock(fieldData);

            // add the Online flag to indicate the new record is from the NISRA data set
            AddCatiManaItems(fieldData);

            _blaiseApi.UpdateCase(existingDataRecord, fieldData,
                instrumentName, serverParkName);
        }

        internal void RemovedCataManaBlock(Dictionary<string, string> fieldData)
        {
            foreach (var f in fieldData
                .Where(kv => kv.Key.StartsWith("CatiMana")).ToList() ) {
                fieldData.Remove(f.Key);
            }
        }

        internal void AddCatiManaItems(Dictionary<string, string> fieldData)
        {
            fieldData.Add("QHAdmin.Online", "1");
        }
    }
}
