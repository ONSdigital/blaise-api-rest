﻿using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.UserRole
{
    public class UserRoleDto :UserAuditInfoDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
