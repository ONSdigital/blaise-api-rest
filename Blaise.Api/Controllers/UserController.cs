using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.User;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILoggingService _loggingService;

        public UserController(
            IUserService userService, 
            ILoggingService loggingService)
        {
            _userService = userService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<UserDto>))]
        public IHttpActionResult GetUsers()
        {
            _loggingService.LogInfo("Getting a list of users");

            var users = _userService.GetUsers();

            _loggingService.LogInfo("Successfully got a list of users");

            return Ok(users);
        }

        [HttpGet]
        [Route("{userName}")]
        [ResponseType(typeof(UserDto))]
        public IHttpActionResult GetUser([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to get user '{userName}'");

            var user = _userService.GetUser(userName);

            _loggingService.LogInfo($"Successfully got user '{userName}'");

            return Ok(user);
        }

        [HttpGet]
        [Route("{userName}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UserExists([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to see if user '{userName}' exists");

            var exists = _userService.UserExists(userName);

            _loggingService.LogInfo($"User '{userName}' exists = '{exists}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult AddUser([FromBody] AddUserDto userDto)
        {
            _loggingService.LogInfo($"Attempting to add user '{userDto.Name}'");

            _userService.AddUser(userDto);

            _loggingService.LogInfo($"Successfully added role '{userDto.Name}'");

            return Created($"{Request.RequestUri}/{userDto.Name}", userDto);
        }

        [HttpDelete]
        [Route("{userName}")]
        public IHttpActionResult RemoveUser([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to remove user '{userName}'");

            _userService.RemoveUser(userName);

            _loggingService.LogInfo($"Successfully removed user '{userName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/password")]
        public IHttpActionResult UpdatePassword([FromUri] string userName, [FromBody] UpdateUserPasswordDto passwordDto)
        {
            _loggingService.LogInfo($"Attempting to update password for user '{userName}'");

            _userService.UpdatePassword(userName, passwordDto);

            _loggingService.LogInfo($"Successfully updated password for user '{userName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/role")]
        public IHttpActionResult UpdateRole([FromUri] string userName, [FromBody] UpdateUserRoleDto roleDto)
        {
            _loggingService.LogInfo($"Attempting to update user '{userName}' role to '{roleDto.Role}'");

            _userService.UpdateRole(userName, roleDto);

            _loggingService.LogInfo($"Successfully updated role for user '{userName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/serverparks")]
        public IHttpActionResult UpdateServerParks([FromUri] string userName, [FromBody] UpdateUserServerParksDto serverParksDto)
        {
            _loggingService.LogInfo($"Attempting to update server parks for user '{userName}'");

            _userService.UpdateServerParks(userName, serverParksDto);

            _loggingService.LogInfo($"Successfully updated server parks for user '{userName}'");

            return NoContent();
        }
    }
}
