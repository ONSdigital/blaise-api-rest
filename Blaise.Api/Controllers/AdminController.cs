using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly ILoggingService _loggingService;

        public AdminController(
            IAdminService adminService,
            ILoggingService loggingService) : base(loggingService)
        {
            _adminService = adminService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("connections")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<OpenConnectionDto>))]
        public IHttpActionResult GetActiveConnections()
        {
            var openConnections = _adminService.GetOpenConnections();

            return Ok(openConnections);
        }

        [HttpDelete]
        [Route("connections")]
        public IHttpActionResult ResetConnections()
        {
            _adminService.ResetConnections();
            _loggingService.LogInfo("reset all cached blaise connections");

            return Ok();
        }
    }
}
