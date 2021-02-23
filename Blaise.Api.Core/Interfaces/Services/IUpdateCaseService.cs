using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IUpdateCaseService
    {
        void UpdateExistingCaseWithOnlineData(IDataRecord nisraDataRecord, IDataRecord existingDataRecord, string serverParkName, 
            string instrumentName, string primaryKey);
    }
}