namespace Blaise.Api.Contracts.Models.UserRole
{
    using System.Collections.Generic;

    public class UserRoleDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
