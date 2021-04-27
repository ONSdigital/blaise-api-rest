using System.Collections.Generic;
using Blaise.Api.Core.Models;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IOnlineCaseService
    {
        IEnumerable<CaseComparisonModel> GetExistingCaseComparisonModels(string instrumentName, string serverParkName);

        bool UpdateExistingCase(int nisraOutcome, int existingOutcome, string nisraUpdatedDate,
            string existingUpdatedDate, string primaryKey, string instrumentName);

        void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string instrumentName,
            string serverParkName, int newOutcome, int existingOutcome, string primaryKey);
    }
}