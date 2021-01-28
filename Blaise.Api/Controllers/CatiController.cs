using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/cati")]
    public class CatiController : BaseController
    {
        private readonly ICatiService _catiService;
        private readonly ILoggingService _loggingService;

        public CatiController(
            ICatiService catiService, 
            ILoggingService loggingService)
        {
            _catiService = catiService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("instruments")]
        [ResponseType(typeof(IEnumerable<CatiInstrumentDto>))]
        public IHttpActionResult GetInstruments()
        {
            _loggingService.LogInfo("Obtaining a list of instruments from Cati");

            var instruments = _catiService.GetCatiInstruments().ToList();

            _loggingService.LogInfo($"Successfully received a list of instruments from Cati '{string.Join(", ", instruments)}'");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments")]
        [ResponseType(typeof(IEnumerable<CatiInstrumentDto>))]
        public IHttpActionResult GetInstruments([FromUri] string serverParkName)
        {
            _loggingService.LogInfo($"Obtaining a list of instruments from Cati for server park '{serverParkName}'");

            var instruments = _catiService.GetCatiInstruments(serverParkName).ToList();

            _loggingService.LogInfo($"Successfully received a list of instruments from Cati '{string.Join(", ", instruments)}'");

            return Ok(instruments);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}")]
        [ResponseType(typeof(CatiInstrumentDto))]
        public IHttpActionResult GetInstrument([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Obtaining an instrument from Cati for server park '{serverParkName}'");

            var instrument = _catiService.GetCatiInstrument(serverParkName, instrumentName);

            _loggingService.LogInfo("Successfully received an instrument from Cati");

            return Ok(instrument);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/daybatch")]
        public IHttpActionResult CreateDaybatch([FromUri] string serverParkName, [FromUri] string instrumentName, [FromBody] DayBatchDto dayBatchDto)
        {
            _loggingService.LogInfo($"Create a daybatch for instrument '{instrumentName}' on server park '{serverParkName}' for '{dayBatchDto.DaybatchDate}'");

            _catiService.CreateDayBatch(instrumentName, serverParkName, dayBatchDto);

            _loggingService.LogInfo($"Daybatch created for instrument '{instrumentName}' on '{dayBatchDto.DaybatchDate}'");

            return Created("", dayBatchDto);
        }
    }
}
