using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ICaseComparisonService _caseComparisonService;
        private readonly IOnlineCaseUpdateService _onlineCaseUpdateService;

        public CaseService(
            IBlaiseCaseApi blaiseApi,
            ICaseComparisonService caseComparisonService,
            IOnlineCaseUpdateService onlineCaseService)
        {
            _blaiseApi = blaiseApi;
            _caseComparisonService = caseComparisonService;
            _onlineCaseUpdateService = onlineCaseService;
        }

        public void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName)
        {
            var existingCaseStatusModelList = _blaiseApi.GetCaseStatusList(instrumentName, serverParkName).ToList();
            var caseRecords = _blaiseApi.GetCases(databaseFilePath);

            while (!caseRecords.EndOfSet)
            {
                var nisraRecord = caseRecords.ActiveRecord;
                var nisraCaseStatusModel = GetNisraCaseStatusModel(nisraRecord);
                var existingCaseStatusModel = GetExistingCaseStatusModel(nisraCaseStatusModel.PrimaryKey, existingCaseStatusModelList);

                if (CaseNeedsToBeUpdated(nisraCaseStatusModel, existingCaseStatusModel, instrumentName))
                {
                    var existingRecord = _blaiseApi.GetCase(nisraCaseStatusModel.PrimaryKey, instrumentName, serverParkName);

                    _onlineCaseUpdateService.UpdateCase(nisraRecord, existingRecord,
                        instrumentName, serverParkName);
                }

                caseRecords.MoveNext();
            }
        }

        private CaseStatusModel GetNisraCaseStatusModel(IDataRecord nisraDataRecord)
        {
            return _blaiseApi.GetCaseStatus(nisraDataRecord);
        }

        private static CaseStatusModel GetExistingCaseStatusModel(string primaryKeyValue, List<CaseStatusModel> existingCaseStatusModelList)
        {
            return existingCaseStatusModelList.FirstOrDefault(t =>
                t.PrimaryKey == primaryKeyValue);
        }
        private bool CaseNeedsToBeUpdated(CaseStatusModel nisraCaseStatusModel, CaseStatusModel existingCaseStatusModel,
                    string instrumentName)
        {
            return existingCaseStatusModel != null && _caseComparisonService.UpdateExistingCase(nisraCaseStatusModel, existingCaseStatusModel,
                instrumentName);
        }


    }
}
