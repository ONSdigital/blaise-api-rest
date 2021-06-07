using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/admin")]
    public class AdminController : BaseController
    {
        private readonly IBlaiseAdminApi _blaiseAdminApi;
        private readonly ILoggingService _loggingService;

        public AdminController(
            IBlaiseAdminApi blaiseAdminApi,
            ILoggingService loggingService) : base(loggingService)
        {
            _loggingService = loggingService;
            _blaiseAdminApi = blaiseAdminApi;
        }

        [HttpGet]
        [Route("connections")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(int))]
        public IHttpActionResult GetActiveConnections()
        {
            return Ok(_blaiseAdminApi.ActiveConnections());
        }

        [HttpDelete]
        [Route("connections")]
        public IHttpActionResult ResetConnections()
        {
            _blaiseAdminApi.ResetConnections();
            _loggingService.LogInfo("reset all cached blaise connections");

            return Ok();
        }
    }
}
