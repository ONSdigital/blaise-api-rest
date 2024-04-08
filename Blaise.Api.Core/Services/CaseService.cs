using System;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.Meta.Constants;

namespace Blaise.Api.Core.Services
{
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
            var primaryKeyValues = new Dictionary<string, string>{ { "QID.Serial_Number", caseId } };
            var caseRecord = _blaiseCaseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

            return _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.PostCode).ValueAsText;
        }

        public CaseDto GetCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
            var caseRecord = _blaiseCaseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

            return new CaseDto
            {
                CaseId = caseId,
                FieldData = _blaiseCaseApi.GetRecordDataFields(caseRecord)
            };
        }

        public CaseMultikeyDto GetCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            keyNames.ThrowExceptionIfNullOrEmpty("keyNames");
            keyValues.ThrowExceptionIfNullOrEmpty("keyValues");

            var primaryKeyValues = new Dictionary<string, string>();
            for (var i = 0; i < keyNames.Count; i++)
            {
                primaryKeyValues.Add(keyNames[i], keyValues[i]);
            }

            var caseRecord = _blaiseCaseApi.GetCase(primaryKeyValues, questionnaireName, serverParkName);

            return new CaseMultikeyDto
            {
                PrimaryKeyValues = primaryKeyValues,
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

            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
            _blaiseCaseApi.CreateCase(primaryKeyValues, fieldData, questionnaireName, serverParkName);
        }

        public void CreateCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues,
            Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");
            keyNames.ThrowExceptionIfNullOrEmpty("keyNames");
            keyValues.ThrowExceptionIfNullOrEmpty("keyValues");

            var primaryKeyValues = new Dictionary<string, string>();
            for (var i = 0; i < keyNames.Count; i++)
            {
                primaryKeyValues.Add(keyNames[i], keyValues[i]);
            }
            _blaiseCaseApi.CreateCase(primaryKeyValues, fieldData, questionnaireName, serverParkName);
        }

        public int CreateCases(List<CaseDto> fieldData, string questionnaireName, string serverParkName)
        {
            _blaiseCaseApi.RemoveCases(questionnaireName, serverParkName);

            // Calculate the number of batches (chunks)
            var batchSize = Properties.Settings.Default.MaxChunkSize;
            var totalItems = fieldData.Count;
            var numBatches = (int)Math.Ceiling((double)totalItems / batchSize);

            for (var batchIndex = 0; batchIndex < numBatches; batchIndex++)
            {
                // Get a chunk of data (batch) for processing
                var caseDtoBatch = fieldData.Skip(batchIndex * batchSize).Take(batchSize).ToList();
                var caseModelList = new List<CaseModel>();

                foreach (var caseDto in caseDtoBatch)
                {
                    var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseDto.CaseId } };
                    caseModelList.Add(new CaseModel(primaryKeyValues, caseDto.FieldData));
                }

                _blaiseCaseApi.CreateCases(caseModelList, questionnaireName, serverParkName);
            }
            return totalItems;
        }

        public void UpdateCase(string serverParkName, string questionnaireName, string caseId, Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");

            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
            _blaiseCaseApi.UpdateCase(primaryKeyValues, fieldData, questionnaireName, serverParkName);
        }


        public void UpdateCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues, Dictionary<string, string> fieldData)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            fieldData.ThrowExceptionIfNullOrEmpty("fieldData");
            keyNames.ThrowExceptionIfNullOrEmpty("keyNames");
            keyValues.ThrowExceptionIfNullOrEmpty("keyValues");


            var primaryKeyValues = new Dictionary<string, string>();
            for (var i = 0; i < keyNames.Count; i++)
            {
                primaryKeyValues.Add(keyNames[i], keyValues[i]);
            }
            _blaiseCaseApi.UpdateCase(primaryKeyValues, fieldData, questionnaireName, serverParkName);
        }

        public void DeleteCase(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
            _blaiseCaseApi.RemoveCase(primaryKeyValues, questionnaireName, serverParkName);
        }

        public void DeleteCase(string serverParkName, string questionnaireName, List<string> keyNames, List<string> keyValues)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            keyNames.ThrowExceptionIfNullOrEmpty("keyNames");
            keyValues.ThrowExceptionIfNullOrEmpty("keyValues");

            var primaryKeyValues = new Dictionary<string, string>();
            for (var i = 0; i < keyNames.Count; i++)
            {
                primaryKeyValues.Add(keyNames[i], keyValues[i]);
            }
            _blaiseCaseApi.RemoveCase(primaryKeyValues, questionnaireName, serverParkName);
        }

        public bool CaseExists(string serverParkName, string questionnaireName, string caseId)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            questionnaireName.ThrowExceptionIfNullOrEmpty("questionnaireName");
            caseId.ThrowExceptionIfNullOrEmpty("caseId");

            var primaryKeyValues = new Dictionary<string, string> { { "QID.Serial_Number", caseId } };
            return _blaiseCaseApi.CaseExists(primaryKeyValues, questionnaireName, serverParkName);
        }
    }
}