using System.Collections.Generic;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface ICaseService
    {
        List<string> GetCaseIds(string serverParkName, string instrumentName);
    }
}