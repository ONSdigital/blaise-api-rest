﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Logging.Services;
using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments")]
    public class InstrumentController : BaseController
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IInstallInstrumentService _installInstrumentService;
        private readonly IUninstallInstrumentService _uninstallInstrumentService;

        public InstrumentController(
            IInstrumentService instrumentService,
            IInstallInstrumentService installInstrumentService, 
            IUninstallInstrumentService uninstallInstrumentService)
        {
            _instrumentService = instrumentService;
            _installInstrumentService = installInstrumentService;
            _uninstallInstrumentService = uninstallInstrumentService;
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IEnumerable<InstrumentDto>))]
        public IHttpActionResult GetInstruments(string serverParkName)
        {
            LoggingService.LogInfo("Obtaining a list of instruments for a server park");

            var instruments = _instrumentService.GetInstruments(serverParkName).ToList();

            LoggingService.LogInfo($"Successfully received a list of instruments '{string.Join(", ", instruments)}'");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("{instrumentName}")]
        [ResponseType(typeof(InstrumentDto))]
        public IHttpActionResult GetInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo("Get an instrument for a server park");

            var instruments = _instrumentService
                .GetInstrument(instrumentName, serverParkName);

            LoggingService.LogInfo($"Successfully retrieved an instrument '{instrumentName}'");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("{instrumentName}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult InstrumentExists([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Check that an instrument exists on server park '{serverParkName}'");

            var exists = _instrumentService.InstrumentExists(instrumentName, serverParkName);

            LoggingService.LogInfo($"Instrument '{instrumentName}' exists = '{exists}' on '{serverParkName}'");

            return Ok(exists);
        }

        [HttpGet]
        [Route("{instrumentName}/id")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult GetInstrumentId([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Get the ID of an instrument on server park '{serverParkName}'");

            var instrumentId = _instrumentService.GetInstrumentId(instrumentName, serverParkName);

            LoggingService.LogInfo($"Instrument ID '{instrumentId}' retrieved");

            return Ok(instrumentId);
        }

        [HttpGet]
        [Route("{instrumentName}/status")]
        [ResponseType(typeof(SurveyStatusType))]
        public IHttpActionResult GetInstrumentStatus([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Get the status of an instrument on server park '{serverParkName}'");

            var status = _instrumentService.GetInstrumentStatus(instrumentName, serverParkName);

            LoggingService.LogInfo($"Instrument '{instrumentName}' has the status '{status}'");

            return Ok(status);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult InstallInstrument([FromUri] string serverParkName, [FromBody] InstallInstrumentDto installInstrumentDto)
        {
            LoggingService.LogInfo($"Attempting to install instrument '{installInstrumentDto.InstrumentFile}' on server park '{serverParkName}'");

            _installInstrumentService.InstallInstrument(serverParkName, installInstrumentDto);

            LoggingService.LogInfo($"Instrument '{installInstrumentDto.InstrumentFile}' installed on server park '{serverParkName}'");

            return Created($"{Request.RequestUri}/{installInstrumentDto.InstrumentName}", installInstrumentDto);
        }

        [HttpDelete]
        [Route("{instrumentName}")]
        public IHttpActionResult UninstallInstrument([FromUri] string serverParkName,[FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Attempting to uninstall instrument '{instrumentName}' on server park '{serverParkName}'");

            _uninstallInstrumentService.UninstallInstrument(instrumentName, serverParkName);

            LoggingService.LogInfo($"Instrument '{instrumentName}' has been uninstalled from server park '{serverParkName}'");

            return NoContent();
        }


        [HttpPatch]
        [Route("{instrumentName}/activate")]
        public IHttpActionResult ActivateInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Activate instrument '{instrumentName}' on server park '{serverParkName}'");

            _instrumentService
                .ActivateInstrument(instrumentName, serverParkName);

            LoggingService.LogInfo($"Successfully activated instrument '{instrumentName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{instrumentName}/deactivate")]
        public IHttpActionResult DeactivateInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Deactivate instrument '{instrumentName}' on server park '{serverParkName}'");

            _instrumentService
                .DeactivateInstrument(instrumentName, serverParkName);

            LoggingService.LogInfo($"Successfully deactivated instrument '{instrumentName}'");

            return NoContent(); 
        }
    }
}
