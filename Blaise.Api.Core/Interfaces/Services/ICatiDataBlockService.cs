﻿using System.Collections.Generic;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICatiDataBlockService
    {
        void RemoveCatiManaBlock(Dictionary<string, string> fieldData);
        void RemoveCallHistoryBlock(Dictionary<string, string> fieldData);
        void RemoveWebNudgedField(Dictionary<string, string> fieldData);

        void AddCatiManaCallItems(Dictionary<string, string> newFieldData, 
            Dictionary<string, string> existingFieldData, int outcomeCode);
    }
}