using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.UserRole;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/userroles")]
    public class UserRoleController : BaseController
    {
        private readonly IUserRoleService _roleService;
        private readonly ILoggingService _loggingService;

        public UserRoleController(
            IUserRoleService roleService,
            ILoggingService loggingService)
        {
            _roleService = roleService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<UserRoleDto>))]
        public IHttpActionResult GetRoles()
        {
            _loggingService.LogInfo("Getting a list of user roles");

            var roles = _roleService.GetUserRoles();

            _loggingService.LogInfo("Successfully got a list of user roles");

            return Ok(roles);
        }

        [HttpGet]
        [Route("{name}")]
        [ResponseType(typeof(UserRoleDto))]
        public IHttpActionResult GetUserRole([FromUri] string name)
        {
            _loggingService.LogInfo($"Attempting to get user role '{name}'");

            var role = _roleService.GetUserRole(name);

            _loggingService.LogInfo($"Successfully got user role '{name}'");

            return Ok(role);
        }

        [HttpGet]
        [Route("{name}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UserRoleExists([FromUri] string name)
        {
            _loggingService.LogInfo($"Attempting to see if user role '{name}' exists");

            var exists = _roleService.UserRoleExists(name);

            _loggingService.LogInfo($"Role '{name}' user exists = '{exists}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult AddUserRole([FromBody] UserRoleDto roleDto)
        {
            _loggingService.LogInfo($"Attempting to add user role '{roleDto.Name}'");
            
            _roleService.AddUserRole(roleDto);
            
            _loggingService.LogInfo($"Successfully added user role '{roleDto.Name}'");

            return Created($"{Request.RequestUri}/{roleDto.Name}", roleDto);
        }

        [HttpDelete]
        [Route("{name}")]
        public IHttpActionResult RemoveUserRole([FromUri] string name)
        {
            _loggingService.LogInfo($"Attempting to remove user role '{name}'");

            _roleService.RemoveUserRole(name);

            _loggingService.LogInfo($"Successfully removed user role '{name}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{name}/permissions")]
        public IHttpActionResult UpdateRolePermissions([FromUri] string name, [FromBody] IEnumerable<string> permissions)
        {
            _loggingService.LogInfo("Attempting to update permissions for user role '{name}'");

            _roleService.UpdateUserRolePermissions(name, permissions);

            _loggingService.LogInfo($"Successfully updated permissions for user role '{name}'");

            return NoContent();
        }
    }
}
