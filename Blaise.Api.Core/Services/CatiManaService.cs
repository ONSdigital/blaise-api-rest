using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Blaise.Api.Core.Interfaces.Services;

[assembly: InternalsVisibleTo("Blaise.Api.Tests.Unit")]
namespace Blaise.Api.Core.Services
{
    public class CatiManaService : ICatiManaService
    {
        public void RemoveCatiManaBlock(Dictionary<string, string> fieldData)
        {
            foreach (var f in fieldData
                .Where(kv => kv.Key.StartsWith("CatiMana")).ToList())
            {
                fieldData.Remove(f.Key);
            }
        }
        
        public void RemoveWebNudgedField(Dictionary<string, string> fieldData)
        {
            if (fieldData.ContainsKey("WebNudged"))
            {
                fieldData.Remove("WebNudged");
            }
        }

        public void AddCatiManaCallItems(Dictionary<string, string> newFieldData, 
            Dictionary<string, string> existingFieldData, int outcomeCode)
        {
            var catiCallItems = BuildCatiManaRegCallItems(existingFieldData, 
                outcomeCode);

            AddCatiManaNrOfCallItem(newFieldData, existingFieldData);

            foreach (var catiCallItem in catiCallItems)
            {
                newFieldData.Add(catiCallItem.Key, catiCallItem.Value);
            }
        }

        internal void AddCatiManaNrOfCallItem(Dictionary<string, string> newFieldData, 
            Dictionary<string, string> existingFieldData)
        {
            newFieldData.Add("CatiMana.CatiCall.NrOfCall",
                int.TryParse(existingFieldData["CatiMana.CatiCall.NrOfCall"], out var numberOfCalls)
                    ? (numberOfCalls + 1).ToString()
                    : "1");
        }

        internal Dictionary<string, string> BuildCatiManaRegCallItems(Dictionary<string, string> fieldData, 
            int outcomeCode)
        {
            var catiCallItems = new Dictionary<string, string>
            {
                {"CatiMana.CatiCall.RegsCalls[1].WhoMade", "Web"},
                {"CatiMana.CatiCall.RegsCalls[1].DayNumber", "1"},
                {"CatiMana.CatiCall.RegsCalls[1].DialTime", $"{DateTime.Now:HH:mm:ss}"},
                {"CatiMana.CatiCall.RegsCalls[1].NrOfDials", "1"},
                {"CatiMana.CatiCall.RegsCalls[1].DialResult", outcomeCode == 110 ? "1" : "2"}
            };

            for (var i = 1; i <= 5; i++)
            {
                foreach (var f in fieldData
                    .Where(kv => kv.Key.StartsWith($"CatiMana.CatiCall.RegsCalls[{i}]")).ToList())
                {
                    var key = f.Key;

                    if (i < 4)
                    {
                        key = key.Replace($"RegsCalls[{i}]", $"RegsCalls[{i + 1}]");
                        catiCallItems.Add(key, f.Value);
                    }

                    if (i == 5)
                    {
                        catiCallItems.Add(key, f.Value);
                    }
                } 
            }

            return catiCallItems;
        }
    }
}
