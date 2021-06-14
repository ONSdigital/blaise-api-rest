using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Admin;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IAdminService
    {
        IEnumerable<OpenConnectionDto> GetOpenConnections();
        void ResetConnections();
    }
}