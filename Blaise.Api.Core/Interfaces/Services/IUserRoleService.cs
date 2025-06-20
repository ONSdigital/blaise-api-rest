using System.Collections.Generic;
using Blaise.Api.Contracts.Models.UserRole;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IUserRoleService
    {
        IEnumerable<UserRoleDto> GetUserRoles();

        UserRoleDto GetUserRole(string name);

        bool UserRoleExists(string name);

        void AddUserRole(UserRoleDto role);

        void RemoveUserRole(string name);

        void UpdateUserRolePermissions(string name, IEnumerable<string> permissions);
    }
}