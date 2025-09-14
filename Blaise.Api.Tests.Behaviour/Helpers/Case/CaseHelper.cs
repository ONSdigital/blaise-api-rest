namespace Blaise.Api.Tests.Behaviour.Helpers.Case
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
    using Blaise.Api.Tests.Behaviour.Helpers.PrimaryKey;
    using Blaise.Api.Tests.Behaviour.Models.Case;
    using Blaise.Api.Tests.Behaviour.Models.Enums;
    using Blaise.Nuget.Api.Api;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Blaise.Nuget.Api.Contracts.Extensions;
    using Blaise.Nuget.Api.Contracts.Interfaces;

    public class CaseHelper
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi = new BlaiseCaseApi();
        private int _primaryKey = 900000;

        private static CaseHelper _currentInstance;

        public static CaseHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new CaseHelper());
        }

        public CaseModel CreateCaseModel(string outCome, ModeType modeType, DateTime lastUpdated)
        {
            return new CaseModel(_primaryKey.ToString(), outCome, modeType, lastUpdated);
        }

        public void CreateCasesInBlaise(int expectedNumberOfCases)
        {
            for (var count = 0; count < expectedNumberOfCases; count++)
            {
                var caseModel = new CaseModel(_primaryKey.ToString(), "110", ModeType.Tel, DateTime.Now.AddHours(-1));
                CreateCaseInBlaise(caseModel);
                _primaryKey++;
            }
        }

        public void CreateCasesInFile(string extractedFilePath, int expectedNumberOfCases)
        {
            for (var count = 0; count < expectedNumberOfCases; count++)
            {
                var caseModel = new CaseModel(_primaryKey.ToString(), "110", ModeType.Web, DateTime.Now.AddMinutes(-20));
                CreateCaseInFile(extractedFilePath, caseModel);
                _primaryKey++;
            }
        }

        public void CreateCasesInFile(string extractedFilePath, IList<CaseModel> caseModels)
        {
            foreach (var caseModel in caseModels)
            {
                caseModel.LastUpdated = DateTime.Now.AddHours(-2);

                CreateCaseInFile(extractedFilePath, caseModel);
            }
        }

        public void CreateCasesInBlaise(IEnumerable<CaseModel> caseModels)
        {
            foreach (var caseModel in caseModels)
            {
                caseModel.LastUpdated = DateTime.Now.AddHours(-1);

                CreateCaseInBlaise(caseModel);
            }
        }

        public void CreateCaseInBlaise(CaseModel caseModel)
        {
            var dataFields = BuildDataFieldsFromCaseModel(caseModel);
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseModel.PrimaryKey);

            _blaiseCaseApi.CreateCase(
                primaryKeys,
                dataFields,
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        public void CreateCaseInFile(string databaseFile, CaseModel caseModel)
        {
            var dataFields = BuildDataFieldsFromCaseModel(caseModel);
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(caseModel.PrimaryKey);

            _blaiseCaseApi.CreateCase(databaseFile, primaryKeys, dataFields);
        }

        public IEnumerable<CaseModel> GetCasesInDatabase()
        {
            var caseModels = new List<CaseModel>();

            var casesInDatabase = _blaiseCaseApi.GetCases(
                BlaiseConfigurationHelper.QuestionnaireName, BlaiseConfigurationHelper.ServerParkName);

            while (!casesInDatabase.EndOfSet)
            {
                var caseRecord = casesInDatabase.ActiveRecord;
                var primaryKey = _blaiseCaseApi.GetPrimaryKeyValues(caseRecord)["QID.Serial_Number"];

                var outcome = _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.HOut).IntegerValue.ToString(CultureInfo.InvariantCulture);
                var mode = _blaiseCaseApi.GetFieldValue(caseRecord, FieldNameType.Mode).EnumerationValue;

                caseModels.Add(new CaseModel(primaryKey, outcome, (ModeType)mode, DateTime.Now));
                casesInDatabase.MoveNext();
            }

            return caseModels;
        }

        public void DeleteCases()
        {
            _blaiseCaseApi.RemoveCases(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        public int NumberOfCasesInQuestionnaire()
        {
            return _blaiseCaseApi.GetNumberOfCases(
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        public ModeType GetMode(string primaryKey)
        {
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(primaryKey);
            var field = _blaiseCaseApi.GetFieldValue(
                primaryKeys,
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName,
                FieldNameType.Mode);

            return (ModeType)field.EnumerationValue;
        }

        public void MarkCaseAsOpenInCati(string primaryKey)
        {
            var primaryKeys = PrimaryKeyHelper.CreatePrimaryKeys(primaryKey);
            var dataRecord = _blaiseCaseApi.GetCase(
                primaryKeys,
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);

            var fieldData = new Dictionary<string, string>
            {
                { FieldNameType.LastUpdatedDate.FullName(), DateTime.Now.ToString("dd-MM-yyyy") },
                { FieldNameType.LastUpdatedTime.FullName(), DateTime.Now.ToString("HH:mm:ss") },
            };

            _blaiseCaseApi.UpdateCase(
                dataRecord,
                fieldData,
                BlaiseConfigurationHelper.QuestionnaireName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        private Dictionary<string, string> BuildDataFieldsFromCaseModel(CaseModel caseModel)
        {
            return new Dictionary<string, string>
            {
                { "SerialNumber", caseModel.PrimaryKey },
                { FieldNameType.HOut.FullName(), caseModel.Outcome },
                { FieldNameType.Mode.FullName(), ((int)caseModel.Mode).ToString() },
                { FieldNameType.LastUpdated.FullName(), caseModel.LastUpdated.ToString("dd-MM-yyyy:HH:mm:ss") },
                { FieldNameType.LastUpdatedDate.FullName(), caseModel.LastUpdated.ToString("dd-MM-yyyy") },
                { FieldNameType.LastUpdatedTime.FullName(), caseModel.LastUpdated.ToString("HH:mm:ss") },
            };
        }
    }
}
