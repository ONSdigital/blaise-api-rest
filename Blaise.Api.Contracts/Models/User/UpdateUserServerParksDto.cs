using System.Collections.Generic;
using System.Linq;

namespace Blaise.Api.Contracts.Models.User
{
    public class UpdateUserServerParksDto : UserAuditInfoDto
    {
        public List<string> ServerParks { get; set; } = new List<string>();

        public string DefaultServerPark => ServerParks.FirstOrDefault();
    }
}
