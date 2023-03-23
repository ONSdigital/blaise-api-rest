using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaise.Api.Core.Mappers
{
    public class AppointmentDto
    {
        public string Questionnaire { get; set; }
        public string ServerPark { get; set; }
        public string PrimaryKey { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string Notes { get; set; } /*Not currently working in Blaise 5.13.3*/
    }
}
