using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Api.Contracts.Models.Reports
{
    public class ReportDto
    {
        public ReportDto()
        {
            ReportingData = new List<Dictionary<string, string>>();
        }

        public List<Dictionary<string, string>> ReportingData { get; set; }
        
    }
}
