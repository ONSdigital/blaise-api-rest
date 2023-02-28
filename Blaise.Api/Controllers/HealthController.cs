using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Health;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/health")]
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

        [HttpGet]
        [Route("BlaiseVersion")]
        [ResponseType(typeof(List<HealthVersionInformationDto>))]
        public IHttpActionResult BlaiseVersion()
        {
            //***************************************************************
            // Load the package assembly
            //***************************************************************
            var assembly = Assembly.Load("StatNeth.Blaise.API");

            //***************************************************************
            // Get the package metadata from the assembly's attributes
            //***************************************************************
            var versionInfo = new HealthVersionInformationDto();
            var metadata = assembly.GetCustomAttributes();

            if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
            {
                versionInfo.Environment = $"Environment: .NET Framework";
            }
            else if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core"))
            {
                versionInfo.Environment = $"Environment: .NET Core";
            }
            else
            {
                versionInfo.Environment = $"Environment: Unknown";
            }

            //***************************************************************
            // Extract the package version from the metadata
            //***************************************************************
            foreach (var attribute in metadata.Where(o => o.TypeId.ToString().Contains("AssemblyFileVersionAttribute"))
                         .ToList())
            {
                if (attribute is AssemblyFileVersionAttribute attrFileVersion)
                {
                    versionInfo.BlaiseVersion = attrFileVersion.Version;
                    versionInfo.DotNetFrameworkVersion = Environment.Version.ToString();
                    break;
                }
            }
            return Ok(versionInfo);
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
