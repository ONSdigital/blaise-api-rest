using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraCaseUpdateService
    {
        void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string questionnaireName, string serverParkName);
    }
}