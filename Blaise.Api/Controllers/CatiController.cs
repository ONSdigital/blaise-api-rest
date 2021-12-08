using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DayBatchDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetDayBatch([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Get a daybatch for instrument '{instrumentName}' on server park '{serverParkName}'");

            var dayBatchDto = _catiService.GetDayBatch(instrumentName, serverParkName);

            _loggingService.LogInfo($"Daybatch retrieved for instrument '{instrumentName}' for '{dayBatchDto.DayBatchDate}'");

            return Ok(dayBatchDto);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch")]
        [SwaggerResponse(HttpStatusCode.Created, Type= typeof(DayBatchDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateDayBatch([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] CreateDayBatchDto createDayBatchDto)
        {
            _loggingService.LogInfo($"Create a daybatch for instrument '{instrumentName}' on server park '{serverParkName}' for '{createDayBatchDto.DayBatchDate}'");

            var dayBatchDto = _catiService.CreateDayBatch(instrumentName, serverParkName, createDayBatchDto);

            _loggingService.LogInfo($"Daybatch created for instrument '{instrumentName}' on '{createDayBatchDto.DayBatchDate}'");

            return Created($"{Request.RequestUri}", dayBatchDto);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch/today")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DayBatchForToday([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Check a daybatch exists today for instrument '{instrumentName}' on server park '{serverParkName}'");

            var exists = _catiService.InstrumentHasADayBatchForToday(instrumentName, serverParkName);

            _loggingService.LogInfo($"Daybatch exists today for instrument '{instrumentName}' - '{exists}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch/add")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateDayBatch([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] List<string> caseIds)
        {
            _loggingService.LogInfo($"Add cases to the current daybatch for instrument '{instrumentName}' on server park '{serverParkName}'");

            _catiService.AddCasesToDayBatch(instrumentName, serverParkName, caseIds);

            _loggingService.LogInfo($"Cases added to daybatch to the current daybatch for instrument '{instrumentName}' on server park '{serverParkName}'");

            return NoContent();
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DateTime>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetSurveyDays([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Get survey days for instrument '{instrumentName}' on server park '{serverParkName}'");

            var surveyDays = _catiService.GetSurveyDays(instrumentName, serverParkName);

            _loggingService.LogInfo($"Survey days retrieved for instrument '{instrumentName}'");

            return Ok(surveyDays);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(List<DateTime>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult AddSurveyDays([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] List<DateTime> surveyDays)
        {
            _loggingService.LogInfo($"Add survey days for instrument '{instrumentName}' on server park '{serverParkName}' for '{surveyDays}'");

            surveyDays = _catiService.AddSurveyDays(instrumentName, serverParkName, surveyDays);

            _loggingService.LogInfo($"Survey days added for instrument '{instrumentName}'");

            return Created($"{Request.RequestUri}", surveyDays);
        }

        [HttpDelete]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult RemoveSurveyDays([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] List<DateTime> surveyDays)
        {
            _loggingService.LogInfo($"Remove survey days for instrument '{instrumentName}' on server park '{serverParkName}' for '{surveyDays}'");

             _catiService.RemoveSurveyDays(instrumentName, serverParkName, surveyDays);

            _loggingService.LogInfo($"Survey days Removed for instrument '{instrumentName}'");

            return NoContent();
        }
    }
}
