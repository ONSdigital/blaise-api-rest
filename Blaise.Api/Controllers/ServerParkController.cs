using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.ServerPark;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

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
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<ServerParkDto>))]
        public IHttpActionResult GetServerParks()
        {
            _loggingService.LogInfo("Obtaining a list of server parks");

            var parks = _serverParkService.GetServerParks().ToList();

            _loggingService.LogInfo($"Successfully received a list of server parks '{string.Join(", ", parks)}'");

            return Ok(parks);
        }

        [HttpGet]
        [Route("{serverParkName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(ServerParkDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetServerPark([FromUri] string serverParkName)
        {
            var park = _serverParkService.GetServerPark(serverParkName);

            _loggingService.LogInfo($"Successfully received server park '{serverParkName}'");

            return Ok(park);
        }

        [HttpGet]
        [Route("{serverParkName}/exists")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        public IHttpActionResult ServerParkExists([FromUri] string serverParkName)
        {
            var exists = _serverParkService.ServerParkExists(serverParkName);

            _loggingService.LogInfo($"Successfully found server park '{serverParkName}'");

            return Ok(exists);
        }
    }
}
