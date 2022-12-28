using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseCaseApiStub : IBlaiseCaseApi
    {
        public bool CaseExists(string primaryKeyValue, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public string GetPrimaryKeyValue(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public IDataSet GetCases(string databaseFile)
        {
            throw new NotImplementedException();
        }

        public IDataSet GetCases(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IDataRecord GetCase(string primaryKeyValue, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IDataRecord GetCase(string primaryKeyValue, string databaseFile)
        {
            throw new NotImplementedException();
        }

        public void CreateCase(string primaryKeyValue, Dictionary<string, string> fieldData, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void CreateCase(IDataRecord dataRecord, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void CreateCase(string databaseFile, string primaryKeyValue, Dictionary<string, string> fieldData)
        {
            throw new NotImplementedException();
        }

        public void UpdateCase(string primaryKeyValue, Dictionary<string, string> fieldData, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void UpdateCase(IDataRecord dataRecord, Dictionary<string, string> fieldData, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void UpdateCase(IDataRecord dataRecord, Dictionary<string, string> fieldData, string databaseFile)
        {
            throw new NotImplementedException();
        }

        public bool FieldExists(string questionnaireName, string serverParkName, FieldNameType fieldNameType)
        {
            throw new NotImplementedException();
        }

        public bool FieldExists(string questionnaireName, string serverParkName, string fieldName)
        {
            throw new NotImplementedException();
        }

        public bool FieldExists(IDataRecord dataRecord, FieldNameType fieldNameType)
        {
            throw new NotImplementedException();
        }

        public IDataValue GetFieldValue(IDataRecord dataRecord, FieldNameType fieldNameType)
        {
            throw new NotImplementedException();
        }

        public IDataValue GetFieldValue(IDataRecord dataRecord, string fieldName)
        {
            throw new NotImplementedException();
        }

        public IDataValue GetFieldValue(string primaryKeyValue, string questionnaireName, string serverParkName,
            FieldNameType fieldNameType)
        {
            throw new NotImplementedException();
        }

        public void RemoveCase(string primaryKeyValue, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public void RemoveCases(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfCases(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfCases(string databaseFile)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetRecordDataFields(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public int GetOutcomeCode(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public void LockDataRecord(string primaryKeyValue, string questionnaireName, string serverParkName, string lockId)
        {
            throw new NotImplementedException();
        }

        public void UnLockDataRecord(string primaryKeyValue, string questionnaireName, string serverParkName, string lockId)
        {
            throw new NotImplementedException();
        }

        public bool DataRecordIsLocked(string primaryKeyValue, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetLastUpdated(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public string GetLastUpdatedAsString(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public bool CaseInUseInCati(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public CaseStatusModel GetCaseStatus(IDataRecord dataRecord)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CaseStatusModel> GetCaseStatusModelList(string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CaseStatusModel> GetCaseStatusModelList(string databaseFile)
        {
            throw new NotImplementedException();
        }

        public CaseModel GetCaseModel(string primaryKeyValue, string questionnaireName, string serverParkName)
        {
            throw new NotImplementedException();
        }
    }
}
