using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Contracts.Models.User;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/users")]
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<UserDto>))]
        public IHttpActionResult GetUsers()
        {
            _loggingService.LogInfo("Getting a list of users");

            var users = _userService.GetUsers();

            _loggingService.LogInfo("Successfully got a list of users");

            return Ok(users);
        }

        [HttpGet]
        [Route("{userName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetUser([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to get user '{userName}'");

            var user = _userService.GetUser(userName);

            _loggingService.LogInfo($"Successfully got user '{userName}'");

            return Ok(user);
        }

        [HttpGet]
        [Route("{userName}/exists")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UserExists([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to see if user '{userName}' exists");

            var exists = _userService.UserExists(userName);

            _loggingService.LogInfo($"User '{userName}' exists = '{exists}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(AddUserDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult AddUser([FromBody] AddUserDto userDto)
        {
            _loggingService.LogInfo($"Attempting to add user '{userDto.Name}' as '{userDto.CurrentlyLoggedInUser}'");

            _userService.AddUser(userDto);

            _loggingService.LogInfo($"Successfully added user '{userDto.Name}' as '{userDto.CurrentlyLoggedInUser}'");

            return Created($"{Request.RequestUri}/{userDto.Name}", userDto);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("{userName}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult RemoveUser([FromUri] string userName)
        {
            _loggingService.LogInfo($"Attempting to remove user '{userName}'");

            _userService.RemoveUser(userName);

            _loggingService.LogInfo($"Successfully removed user '{userName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/password")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UpdatePassword([FromUri] string userName, [FromBody] UserPasswordDto userPasswordDto)
        {
            _loggingService.LogInfo($"Attempting to update password for user '{userName}' as '{userPasswordDto.CurrentlyLoggedInUser}'");

            _userService.UpdatePassword(userName, userPasswordDto.Password);

            _loggingService.LogInfo($"Successfully updated password for user '{userName}' as '{userPasswordDto.CurrentlyLoggedInUser}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/role")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UpdateRole([FromUri] string userName, [FromBody] UpdateUserRoleDto roleDto)
        {
            _loggingService.LogInfo($"Attempting to update user '{userName}' role to '{roleDto.Role}' as '{roleDto.CurrentlyLoggedInUser}'");

            _userService.UpdateRole(userName, roleDto);

            _loggingService.LogInfo($"Successfully updated role for user '{userName}' as '{roleDto.CurrentlyLoggedInUser}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{userName}/serverparks")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UpdateServerParks([FromUri] string userName, [FromBody] UpdateUserServerParksDto serverParksDto)
        {
            _loggingService.LogInfo($"Attempting to update server parks for user '{userName}' as '{serverParksDto.CurrentlyLoggedInUser}'");

            _userService.UpdateServerParks(userName, serverParksDto);

            _loggingService.LogInfo($"Successfully updated server parks for user '{userName}' as '{serverParksDto.CurrentlyLoggedInUser}'");

            return NoContent();
        }

        [HttpPost]
        [Route("{userName}/validate")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult ValidateUser([FromUri] string userName, [FromBody] UserPasswordDto userPasswordDto)
        {
            _loggingService.LogInfo($"Attempting to validate user '{userName}' credentials");

            var valid = _userService.ValidateUser(userName, userPasswordDto.Password);

            _loggingService.LogInfo($"User '{userName}' credentials have been validated");

            return Ok(valid);
        }
    }
}
