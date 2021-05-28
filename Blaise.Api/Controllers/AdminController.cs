using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Interfaces;

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
