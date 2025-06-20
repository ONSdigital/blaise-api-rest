using System.Collections.Generic;
using Blaise.Api.Contracts.Models.UserRole;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IBlaiseRoleApi _blaiseApi;
        private readonly IUserRoleDtoMapper _dtoMapper;

        public UserRoleService(
            IBlaiseRoleApi blaiseApi, 
            IUserRoleDtoMapper dtoMapper)
        {
            _blaiseApi = blaiseApi;
            _dtoMapper = dtoMapper;
        }

        public IEnumerable<UserRoleDto> GetUserRoles()
        {
            var roles = _blaiseApi.GetRoles();

            return _dtoMapper.MapToUserRoleDtos(roles);
        }

        public UserRoleDto GetUserRole(string name)
        {
            name.ThrowExceptionIfNullOrEmpty("name");
            
            var role = _blaiseApi.GetRole(name);

            return _dtoMapper.MapToUserRoleDto(role);
        }

        public bool UserRoleExists(string name)
        {
            name.ThrowExceptionIfNullOrEmpty("name");

            return _blaiseApi.RoleExists(name);
        }

        public void AddUserRole(UserRoleDto role)
        {
            role.Name.ThrowExceptionIfNullOrEmpty("UserRoleDto.Name");

            _blaiseApi.AddRole(role.Name, role.Description, role.Permissions);
        }

        public void RemoveUserRole(string name)
        {
            name.ThrowExceptionIfNullOrEmpty("name");

            _blaiseApi.RemoveRole(name);
        }

        public void UpdateUserRolePermissions(string name, IEnumerable<string> permissions)
        {
            name.ThrowExceptionIfNullOrEmpty("name");

            _blaiseApi.UpdateRolePermissions(name, permissions);
        }
    }
}
