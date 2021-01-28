using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks")]
    public class ServerParkController : BaseController
    {
        private readonly IServerParkService _serverParkService;
        private readonly ILoggingService _loggingService;

        public ServerParkController(
            IServerParkService parkService,
            ILoggingService loggingService)
        {
            _serverParkService = parkService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<ServerParkDto>))]
        public IHttpActionResult GetServerParks()
        {
            _loggingService.LogInfo("Obtaining a list of server parks");

            var parks = _serverParkService.GetServerParks().ToList();

            _loggingService.LogInfo($"Successfully received a list of server parks '{string.Join(", ", parks)}'");

            return Ok(parks);
        }

        [HttpGet]
        [Route("{serverParkName}")]
        [ResponseType(typeof(ServerParkDto))]
        public IHttpActionResult GetServerPark([FromUri] string serverParkName)
        {
            var park = _serverParkService.GetServerPark(serverParkName);

            _loggingService.LogInfo($"Successfully received server park '{serverParkName}'");

            return Ok(park);
        }

        [HttpGet]
        [Route("{serverParkName}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ServerParkExists([FromUri] string serverParkName)
        {
            var exists = _serverParkService.ServerParkExists(serverParkName);

            _loggingService.LogInfo($"Successfully found server park '{serverParkName}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("{serverParkName}/server")]
        public IHttpActionResult RegisterMachine([FromUri] string serverParkName, [FromBody] ServerDto serverDto)
        {
            _loggingService.LogInfo($"Attempt to register a server '{serverDto.Name}'");

            _serverParkService.RegisterServerOnServerPark(serverParkName, serverDto);

            _loggingService.LogInfo($"Successfully registered a server '{serverDto.Name}'");

            return NoContent();
        }
    }
}
