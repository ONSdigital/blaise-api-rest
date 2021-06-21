using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Admin
{
    public class OpenConnectionDto
    {
        public string ConnectionType { get; set; }

        public int NumberOfConnections { get; set; }

        public Dictionary<string, DateTime> Connections { get; set; }
    }
}
