using System.Collections.Generic;
using Blaise.Api.Contracts.Models.UserRole;
using StatNeth.Blaise.API.Security;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IUserRoleDtoMapper
    {
        IEnumerable<UserRoleDto> MapToUserRoleDtos(IEnumerable<IRole> roles);
        UserRoleDto MapToUserRoleDto(IRole role);
    }
}