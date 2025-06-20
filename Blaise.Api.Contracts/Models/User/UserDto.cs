using System.Collections.Generic;
using System.Linq;

namespace Blaise.Api.Contracts.Models.User
{
    public class UserDto 
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public List<string> ServerParks { get; set; }

        public string DefaultServerPark => ServerParks.FirstOrDefault();
    }
}
