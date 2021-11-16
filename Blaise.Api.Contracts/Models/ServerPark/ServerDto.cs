
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.ServerPark
{
    public class ServerDto
    {
        public string Name { get; set; }

        public string LogicalServerName { get; set; }

        public string BlaiseVersion { get; set; }

        public IList<string> Roles { get; set; }
    }
}
