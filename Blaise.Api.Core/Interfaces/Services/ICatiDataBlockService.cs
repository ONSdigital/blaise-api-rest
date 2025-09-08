namespace Blaise.Api.Core.Interfaces.Services
{
    using System.Collections.Generic;

    public interface ICatiDataBlockService
    {
        void RemoveCatiManaBlock(Dictionary<string, string> fieldData);

        void RemoveCallHistoryBlock(Dictionary<string, string> fieldData);

        void RemoveWebNudgedField(Dictionary<string, string> fieldData);

        void AddCatiManaCallItems(
            Dictionary<string, string> newFieldData,
            Dictionary<string, string> existingFieldData,
            int outcomeCode);
    }
}
