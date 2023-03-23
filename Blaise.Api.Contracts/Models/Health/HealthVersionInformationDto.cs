using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Api.Contracts.Models.Health
{
    public class HealthVersionInformationDto
    {
        public string BlaiseVersion { get; set; }
        public string DotNetFrameworkVersion { get; set; }
        public string Environment { get; set; }
    }
}
