using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly IOnlineCaseService _onlineCaseService;
        private readonly ILoggingService _loggingService;

        public CaseService(
            IBlaiseCaseApi blaiseApi, 
            IOnlineCaseService updateCaseService, 
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _onlineCaseService = updateCaseService;
            _loggingService = loggingService;
        }

        public void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName)
        {
            var caseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!caseRecords.EndOfSet)
            {
                var newRecord = caseRecords.ActiveRecord;
                var primaryKey = _blaiseApi.GetPrimaryKeyValue(newRecord);

                if (_blaiseApi.CaseExists(primaryKey, instrumentName, serverParkName))
                {
                    _loggingService.LogInfo($"Case with serial number '{primaryKey}' exists in Blaise");

                    var existingCase = _blaiseApi.GetCase(primaryKey, instrumentName, serverParkName);
                    _onlineCaseService.UpdateExistingCaseWithOnlineData(newRecord, existingCase, 
                        serverParkName, instrumentName,  primaryKey);
                }
                else
                {
                    _loggingService.LogInfo($"Case with serial number '{primaryKey}' does not exist in Blaise");

                    _onlineCaseService.CreateOnlineCase(newRecord, instrumentName, 
                        serverParkName, primaryKey);
                }

                caseRecords.MoveNext();
            }
        }
    }
}
