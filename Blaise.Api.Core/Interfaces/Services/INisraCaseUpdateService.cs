namespace Blaise.Api.Core.Interfaces.Services
{
    using StatNeth.Blaise.API.DataRecord;

    public interface INisraCaseUpdateService
    {
        void UpdateCase(IDataRecord newDataRecord, IDataRecord existingDataRecord, string questionnaireName, string serverParkName);
    }
}
