namespace Blaise.Api.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Blaise.Nuget.Api.Contracts.Models;
    using StatNeth.Blaise.API.DataRecord;

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
            var existingTelCaseStatusModels = _blaiseApi.GetCaseStatusModelList(questionnaireName, serverParkName).ToList();
            var nisraFileCaseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!nisraFileCaseRecords.EndOfSet)
            {
                var nisraRecord = nisraFileCaseRecords.ActiveRecord;

                var nisraCaseStatusModel = GetNisraCaseStatusModel(nisraRecord);
                var existingCaseStatusModel = GetExistingTelCaseStatusModel(nisraCaseStatusModel.PrimaryKey, existingTelCaseStatusModels);

                if (CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, questionnaireName))
                {
                    var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", nisraCaseStatusModel.PrimaryKey } };
                    var existingRecord = _blaiseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

                    _onlineCaseUpdateService.UpdateCase(
                        nisraRecord,
                        existingRecord,
                        questionnaireName,
                        serverParkName);
                }

                nisraFileCaseRecords.MoveNext();
            }
        }

        private static CaseStatusModel GetExistingTelCaseStatusModel(string primaryKeyValue, IEnumerable<CaseStatusModel> existingCaseStatusModelList)
        {
            return existingCaseStatusModelList.FirstOrDefault(t =>
                t.PrimaryKey == primaryKeyValue);
        }

        private CaseStatusModel GetNisraCaseStatusModel(IDataRecord nisraDataRecord)
        {
            return _blaiseApi.GetCaseStatus(nisraDataRecord);
        }

        private bool CaseNeedsToBeUpdated(
            CaseStatusModel nisraCaseStatusModel,
            CaseStatusModel existingCaseStatusModel,
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
