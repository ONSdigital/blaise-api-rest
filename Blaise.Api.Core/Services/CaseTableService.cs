using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Enums;
using Blaise.Api.Core.Models;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class CaseTableService
    {
        private readonly IBlaiseCaseApi _blaiseApi;

        public CaseTableService(IBlaiseCaseApi blaiseApi)
        {
            _blaiseApi = blaiseApi;
        }

        public IEnumerable<TableCaseModel> GetTelTable(string instrumentName, string serverParkName)
        {
            var tableMap = new List<TableCaseModel>();
            var cases = _blaiseApi.GetCases(instrumentName, serverParkName);

            while (!cases.EndOfSet)
            {
                var record = cases.ActiveRecord;
                var outcomeCode = _blaiseApi.GetOutcomeCode(record);

                if (outcomeCode != 0)
                {
                    tableMap.Add(new TableCaseModel(
                        _blaiseApi.GetPrimaryKeyValue(record),
                        outcomeCode,
                        ModeType.Tel,
                        _blaiseApi.GetFieldValue(record, FieldNameType.LastUpdated)?.ValueAsText
                    ));
                }


                cases.MoveNext();
            }

            return tableMap;
        }
    }
}
