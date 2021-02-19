using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
            Dictionary<string, string> existingFieldData)
        {
            var catiCallItems = BuildCatiManaCallItems(existingFieldData, "", "", 
                "", "", "");

            foreach (var catiCallItem in catiCallItems)
            {
                newFieldData.Add(catiCallItem.Key, catiCallItem.Value);
            }
        }

        internal Dictionary<string, string> BuildCompleteCatiManaCallItem(Dictionary<string, string> fieldData)
        {
            return BuildCatiManaCallItems(fieldData, "", "", 
                "", "", "");
        }

        internal Dictionary<string, string> BuildPartialCatiManaCallItem(Dictionary<string, string> fieldData)
        {
            return BuildCatiManaCallItems(fieldData, "", "", 
                "", "", "");
        }

        internal Dictionary<string, string> BuildCatiManaCallItems(Dictionary<string, string> fieldData, string whoMade, 
            string dayNumber, string dialTime, string numberOfDials, string dialResult)
        {
            var catiCallItems = new Dictionary<string, string>
            {
                {"CatiMana.CatiCall.RegsCalls[1].WhoMade", whoMade},
                {"CatiMana.CatiCall.RegsCalls[1].DayNumber", dayNumber},
                {"CatiMana.CatiCall.RegsCalls[1].DialTime", dialTime},
                {"CatiMana.CatiCall.RegsCalls[1].NrOfDials", numberOfDials},
                {"CatiMana.CatiCall.RegsCalls[1].DialResult", dialResult},
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
