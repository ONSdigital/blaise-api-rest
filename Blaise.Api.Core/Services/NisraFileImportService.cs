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

        public void ImportNisraDatabaseFile(string databaseFilePath, string questionnaireName, string serverParkName)
        {
            _loggerService.LogInfo("NisraFileImportService: ImportNisraDatabaseFile - start");
            var existingTelCaseStatusModels = _blaiseApi.GetCaseStatusModelList(questionnaireName, serverParkName).ToList();
            _loggerService.LogInfo("NisraFileImportService: ImportNisraDatabaseFile - GetCaseStatusModelList");
            var nisraFileCaseRecords = _blaiseApi.GetCases(databaseFilePath);
            _loggerService.LogInfo("NisraFileImportService: ImportNisraDatabaseFile - GetCases (Nisra)");

            while (!nisraFileCaseRecords.EndOfSet)
            {
                _loggerService.LogInfo("NisraFileImportService: Read first Nisra file");
                var nisraRecord = nisraFileCaseRecords.ActiveRecord;
                _loggerService.LogInfo("NisraFileImportService: Got active record");

                var nisraCaseStatusModel = GetNisraCaseStatusModel(nisraRecord);
                _loggerService.LogInfo("NisraFileImportService: GetNisraCaseStatusModel");
                var existingCaseStatusModel = GetExistingTelCaseStatusModel(nisraCaseStatusModel.PrimaryKey, existingTelCaseStatusModels);
                _loggerService.LogInfo("NisraFileImportService: existingCaseStatusModel");

                if (CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, questionnaireName))
                {
                    var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", nisraCaseStatusModel.PrimaryKey } };
                    var existingRecord = _blaiseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

                    _onlineCaseUpdateService.UpdateCase(nisraRecord, existingRecord,
                        questionnaireName, serverParkName);
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
                    string questionnaireName)
        {
            if (existingCaseStatusModel == null)
            {
                _loggerService.LogWarn($"The nisra case '{nisraCaseStatusModel.PrimaryKey}' does not exist in the database for the questionnaire '{questionnaireName}'");
                return false;
            }

            return _caseComparisonService.CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, questionnaireName);
        }
    }
}
