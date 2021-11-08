using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
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
            ILoggingService loggingService) : base(loggingService)
        {
            _healthService = healthService;
            _loggingService = loggingService;
        }
        
        [HttpGet]
        [Route("")]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet]
        [Route("diagnosis")]
        [ResponseType(typeof(List<HealthCheckResultDto>))]
        public IHttpActionResult HealthCheckDiagnosis()
        {
            var results = PerformHealthCheck();

            return Ok(results);
        }

        private List<HealthCheckResultDto> PerformHealthCheck()
        {
            var timer = new Stopwatch();
            timer.Start();
            var healthCheckResults = _healthService.PerformCheck().ToList();
            timer.Stop();

            var timeTookInSeconds = timer.ElapsedMilliseconds / 1000;

            if (timeTookInSeconds > 5)
            {
                _loggingService.LogWarn($"Health check took '{timeTookInSeconds}' seconds");
            }
            else
            {
                _loggingService.LogInfo($"Health check took '{timeTookInSeconds}' seconds");
            }

            return healthCheckResults;
        }
    }
}
