using System.Collections.Generic;
using Blaise.Api.Tests.Helpers.Configuration;
using Blaise.Nuget.Api.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Tests.Helpers.Case
{
    public class CaseHelper
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        private static CaseHelper _currentInstance;

        public CaseHelper()
        {
            _blaiseCaseApi = new BlaiseCaseApi();
        }

        public static CaseHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new CaseHelper());
        }

        public void CreateCases(int expectedNumberOfCases)
        {
            var primaryKey = 9000000;

            for (var count = 0; count < expectedNumberOfCases; count++)
            {
                var dataFields = new Dictionary<string, string> {{"SerialNumber", primaryKey.ToString()}};

                _blaiseCaseApi.CreateCase(primaryKey.ToString(), dataFields, 
                    BlaiseConfigurationHelper.InstrumentName, BlaiseConfigurationHelper.ServerParkName);
                primaryKey++;
            }
        }

        public void DeleteCases()
        {
            var cases = _blaiseCaseApi.GetCases(BlaiseConfigurationHelper.InstrumentName, 
                BlaiseConfigurationHelper.ServerParkName);

            while (!cases.EndOfSet)
            {
                var primaryKey = _blaiseCaseApi.GetPrimaryKeyValue(cases.ActiveRecord);

                _blaiseCaseApi.RemoveCase(primaryKey, BlaiseConfigurationHelper.InstrumentName, 
                    BlaiseConfigurationHelper.ServerParkName);

                cases.MoveNext();
            }
        }

        public int NumberOfCasesInInstrument()
        {
            return _blaiseCaseApi.GetNumberOfCases(BlaiseConfigurationHelper.InstrumentName, 
                BlaiseConfigurationHelper.ServerParkName);
        }

        public object NumberOfCasesInInstrument(string dataInterfaceFile)
        {
            return _blaiseCaseApi.GetNumberOfCases(dataInterfaceFile);
        }
    }
}
