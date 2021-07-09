using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Extensions;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments")]
    public class InstrumentController : BaseController
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IInstrumentInstallerService _installInstrumentService;
        private readonly IInstrumentUninstallerService _uninstallInstrumentService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public InstrumentController(
            IInstrumentService instrumentService,
            IInstrumentInstallerService installInstrumentService,
            IInstrumentUninstallerService uninstallInstrumentService,
            ILoggingService loggingService, IConfigurationProvider configurationProvider) : base(loggingService)
        {
            _instrumentService = instrumentService;
            _installInstrumentService = installInstrumentService;
            _uninstallInstrumentService = uninstallInstrumentService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<InstrumentDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstruments(string serverParkName)
        {
            _loggingService.LogInfo("Obtaining a list of instruments for a server park");

            var instruments = _instrumentService.GetInstruments(serverParkName).ToList();

            _loggingService.LogInfo($"Successfully received {instruments.Count} instruments");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("{instrumentName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(InstrumentDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo("Get an instrument for a server park");

            var instruments = _instrumentService
                .GetInstrument(instrumentName, serverParkName);

            _loggingService.LogInfo($"Successfully retrieved an instrument '{instrumentName}'");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("{instrumentName}/exists")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult InstrumentExists([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Check that an instrument exists on server park '{serverParkName}'");

            var exists = _instrumentService.InstrumentExists(instrumentName, serverParkName);

            _loggingService.LogInfo($"Instrument '{instrumentName}' exists = '{exists}' on '{serverParkName}'");

            return Ok(exists);
        }

        [HttpGet]
        [Route("{instrumentName}/id")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(Guid))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstrumentId([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Get the ID of an instrument on server park '{serverParkName}'");

            var instrumentId = _instrumentService.GetInstrumentId(instrumentName, serverParkName);

            _loggingService.LogInfo($"Instrument ID '{instrumentId}' retrieved");

            return Ok(instrumentId);
        }

        [HttpGet]
        [Route("{instrumentName}/status")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(SurveyStatusType))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstrumentStatus([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Get the status of an instrument on server park '{serverParkName}'");

            var status = _instrumentService.GetInstrumentStatus(instrumentName, serverParkName);

            _loggingService.LogInfo($"Instrument '{instrumentName}' has the status '{status}'");

            return Ok(status);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type=typeof(InstrumentPackageDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public async Task<IHttpActionResult> InstallInstrument([FromUri] string serverParkName, [FromBody] InstrumentPackageDto instrumentPackageDto)
        {
            var tempPath = _configurationProvider.TempPath;
            
            try
            {
                _loggingService.LogInfo($"Attempting to install instrument '{instrumentPackageDto.InstrumentFile}' on server park '{serverParkName}'");

                var instrumentName = await _installInstrumentService.InstallInstrumentAsync(serverParkName, instrumentPackageDto, tempPath);

                _loggingService.LogInfo($"Instrument '{instrumentPackageDto.InstrumentFile}' installed on server park '{serverParkName}'");

                return Created($"{Request.RequestUri}/{instrumentName}", instrumentPackageDto);
            }
            finally
            {
                tempPath.CleanUpTempFiles();
                _loggingService.LogInfo($"Removed temporary files and folder '{tempPath}'");
            }
        }

        [HttpDelete]
        [Route("{instrumentName}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UninstallInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Attempting to uninstall instrument '{instrumentName}' on server park '{serverParkName}'");

            _uninstallInstrumentService.UninstallInstrument(instrumentName, serverParkName);

            _loggingService.LogInfo($"Instrument '{instrumentName}' has been uninstalled from server park '{serverParkName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{instrumentName}/activate")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult ActivateInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Activate instrument '{instrumentName}' on server park '{serverParkName}'");

            _instrumentService
                .ActivateInstrument(instrumentName, serverParkName);

            _loggingService.LogInfo($"Successfully activated instrument '{instrumentName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{instrumentName}/deactivate")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DeactivateInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Deactivate instrument '{instrumentName}' on server park '{serverParkName}'");

            _instrumentService
                .DeactivateInstrument(instrumentName, serverParkName);

            _loggingService.LogInfo($"Successfully deactivated instrument '{instrumentName}'");

            return NoContent(); 
        }
    }
}
