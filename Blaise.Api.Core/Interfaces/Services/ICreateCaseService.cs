using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICreateCaseService
    {
        void CreateOnlineCase(IDataRecord dataRecord, string serverParkName, 
            string instrumentName, string primaryKey);
    }
}