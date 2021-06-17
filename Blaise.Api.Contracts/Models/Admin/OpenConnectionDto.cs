using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Admin
{
    public class OpenConnectionDto
    {
        public string ConnectionType { get; set; }

        public int Connections { get; set; }

        public IEnumerable<DateTime> ExpirationDateTimes { get; set; }
    }
}
