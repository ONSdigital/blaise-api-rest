using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Blaise.Api.Core.Services
{
    using Blaise.Nuget.Api.Contracts.Models;
    using System;

    public class CaseService : ICaseService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public CaseService(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }

        public List<string> GetCaseIds(string serverParkName, string questionnaireName)
        {
            var caseStatusList = _blaiseCaseApi.GetCaseStatusModelList(questionnaireName, serverParkName);

            return caseStatusList.Select(caseStatus => caseStatus.PrimaryKey).ToList();
        }

        public List<CaseStatusDto> GetCaseStatusList(string serverParkName, string questionnaireName)
        {
            var caseStatusList = _blaiseCaseApi.GetCaseStatusModelList(questionnaireName, serverParkName);
            var caseStatusDtoList = new List<CaseStatusDto>();

            foreach (var caseStatus in caseStatusList)
            {
                caseStatusDtoList.Add(

                    new CaseStatusDto
                    {
                        PrimaryKey = caseStatus.PrimaryKey,
                        Outcome = caseStatus.Outcome
                    });

            }

            return caseStatusDtoList;
        }

        public string GetPostCode(string serverParkName, string questionnaireName, string caseId)
        {
            var caseRecord = _blaiseCaseApi.GetCase(caseId, questionnaireName, serverParkName);

            return _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.PostCode).ValueAsText;
        }

        public CaseDto GetCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var caseRecord = _blaiseCaseApi.GetCase(caseId, questionnaireName, serverParkName);

            return new CaseDto
            {
                CaseId = _blaiseCaseApi.GetPrimaryKeyValue(caseRecord),
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public void CreateCase(string serverParkName, string questionnaireName, string caseId,
            Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            _blaiseCaseApi.CreateCase(caseId, fieldData, questionnaireName, serverParkName);
        }

        public int CreateCases(List<CaseModel> fieldData, string questionnaireName, string serverParkName)
        {
            _blaiseCaseApi.RemoveCases(questionnaireName, serverParkName);

            // Calculate the number of batches (chunks)
            var batchSize = Properties.Settings.Default.MaxChunkSize;
            var totalItems = fieldData.Count;
            var numBatches = (int)Math.Ceiling((double)totalItems / batchSize);

            for (var batchIndex = 0; batchIndex < numBatches; batchIndex++)
            {
                // Get a chunk of data (batch) for processing
                var batch = fieldData.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                _blaiseCaseApi.CreateCases(batch, questionnaireName, serverParkName);
                Console.WriteLine($"Total cases written {batch.Count}");
            }
            return totalItems;
        }

        public void UpdateCase(string serverParkName, string questionnaireName, string caseId, Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            _blaiseCaseApi.UpdateCase(caseId, fieldData, questionnaireName, serverParkName);
        }

        public void DeleteCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            _blaiseCaseApi.RemoveCase(caseId, questionnaireName, serverParkName);
        }

        public bool CaseExists(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            return _blaiseCaseApi.CaseExists(caseId, questionnaireName, serverParkName);
        }
    }
}