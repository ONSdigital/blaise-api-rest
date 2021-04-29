using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class NisraFileImportService : INisraFileImportService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly INisraCaseComparisonService _caseComparisonService;
        private readonly INisraCaseUpdateService _onlineCaseUpdateService;
        private readonly ILoggingService _loggerService;

        public NisraFileImportService(
            IBlaiseCaseApi blaiseApi,
            INisraCaseComparisonService caseComparisonService,
            INisraCaseUpdateService onlineCaseService, 
            ILoggingService loggerService)
        {
            _blaiseApi = blaiseApi;
            _caseComparisonService = caseComparisonService;
            _onlineCaseUpdateService = onlineCaseService;
            _loggerService = loggerService;
        }

        public void ImportNisraDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName)
        {
            var existingTelCaseStatusModels = _blaiseApi.GetCaseStatusList(instrumentName, serverParkName).ToList();
            var nisraFileCaseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!nisraFileCaseRecords.EndOfSet)
            {
                var nisraRecord = nisraFileCaseRecords.ActiveRecord;

                var nisraCaseStatusModel = GetNisraCaseStatusModel(nisraRecord);
                var existingCaseStatusModel = GetExistingTelCaseStatusModel(nisraCaseStatusModel.PrimaryKey, existingTelCaseStatusModels);

                if (CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, instrumentName))
                {
                    var existingRecord = _blaiseApi.GetCase(nisraCaseStatusModel.PrimaryKey, instrumentName, serverParkName);

                    _onlineCaseUpdateService.UpdateCase(nisraRecord, existingRecord,
                        instrumentName, serverParkName);
                }

                nisraFileCaseRecords.MoveNext();
            }
        }

        private CaseStatusModel GetNisraCaseStatusModel(IDataRecord nisraDataRecord)
        {
            return _blaiseApi.GetCaseStatus(nisraDataRecord);
        }

        private static CaseStatusModel GetExistingTelCaseStatusModel(string primaryKeyValue, IEnumerable<CaseStatusModel> existingCaseStatusModelList)
        {
            return existingCaseStatusModelList.FirstOrDefault(t =>
                t.PrimaryKey == primaryKeyValue);
        }
        private bool CaseNeedsToBeUpdated(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel,
                    string instrumentName)
        {
            if (existingCaseStatusModel == null)
            {
                _loggerService.LogWarn($"The nisra case '{nisraCaseStatusModel.PrimaryKey}' does not exist in the database for the instrument '{instrumentName}'");
            }

            return _caseComparisonService.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, instrumentName);
        }
    }
}
