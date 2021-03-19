﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/cati")]
    public class CatiController : BaseController
    {
        private readonly ICatiService _catiService;
        private readonly ILoggingService _loggingService;

        public CatiController(
            ICatiService catiService, 
            ILoggingService loggingService) : base(loggingService)
        {
            _catiService = catiService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("instruments")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<CatiInstrumentDto>))]
        public IHttpActionResult GetInstruments()
        {
            _loggingService.LogInfo("Obtaining a list of instruments from Cati");

            var instruments = _catiService.GetCatiInstruments().ToList();

            _loggingService.LogInfo($"Successfully received {instruments.Count} instruments from Cati");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<CatiInstrumentDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstruments([FromUri] string serverParkName)
        {
            _loggingService.LogInfo($"Obtaining a list of instruments from Cati for server park '{serverParkName}'");

            var instruments = _catiService.GetCatiInstruments(serverParkName).ToList();

            _loggingService.LogInfo($"Successfully received {instruments.Count} instruments from Cati");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(CatiInstrumentDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Obtaining an instrument from Cati for server park '{serverParkName}'");

            var instrument = _catiService.GetCatiInstrument(serverParkName, instrumentName);

            _loggingService.LogInfo("Successfully received an instrument from Cati");

            return Ok(instrument);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch")]
        [SwaggerResponse(HttpStatusCode.Created, Type= typeof(DayBatchDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateDaybatch([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] DayBatchDto dayBatchDto)
        {
            _loggingService.LogInfo($"Create a daybatch for instrument '{instrumentName}' on server park '{serverParkName}' for '{dayBatchDto.DaybatchDate}'");

            _catiService.CreateDayBatch(instrumentName, serverParkName, dayBatchDto);

            _loggingService.LogInfo($"Daybatch created for instrument '{instrumentName}' on '{dayBatchDto.DaybatchDate}'");

            return Created("", dayBatchDto);
        }
    }
}
