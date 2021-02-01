using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Enums;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Health;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/health")]
    public class HealthController : BaseController
    {
        private readonly IHealthCheckService _healthService;
        private readonly ILoggingService _loggingService;

        public HealthController(
            IHealthCheckService healthService,
            ILoggingService loggingService)
        {
            _healthService = healthService;
            _loggingService = loggingService;
        }
        
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<HealthCheckResultDto>))]
        public IHttpActionResult HealthCheck()
        {
            _loggingService.LogInfo("performing Health check on Blaise connectivity");

            var results = _healthService.PerformCheck().ToList();

            if (results.Any(r => r.StatusType == HealthStatusType.Error))
            {
                return Content(HttpStatusCode.ServiceUnavailable, results);
            }

            return Ok(results);
        }
    }
}
