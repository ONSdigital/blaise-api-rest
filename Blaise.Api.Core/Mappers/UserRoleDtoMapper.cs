using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.UserRole;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.Security;

namespace Blaise.Api.Core.Mappers
{
    public class UserRoleDtoMapper : IUserRoleDtoMapper
    {
        public IEnumerable<UserRoleDto> MapToUserRoleDtos(IEnumerable<IRole> roles)
        {
            var roleDtos = new List<UserRoleDto>();

            foreach (var role in roles)
            {
                roleDtos.Add(MapToUserRoleDto(role));
            }

            return roleDtos;
        }

        public UserRoleDto MapToUserRoleDto(IRole role)
        {
            return new UserRoleDto
            {
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
                    .Where(p => p.Permission == PermissionStatus.Allowed)
                    .Select(p => p.Action)
            };
        }
    }
}
