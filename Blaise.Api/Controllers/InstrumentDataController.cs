using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/{instrumentName}/data")]
    public class InstrumentDataController : BaseController
    {
        private readonly IInstrumentDataService _instrumentDataService;
        private readonly ILoggingService _loggingService;
        
        public InstrumentDataController(
            IInstrumentDataService dataDeliveryService, 
            ILoggingService loggingService)
        {
            _instrumentDataService = dataDeliveryService;
            _loggingService = loggingService;
        }

        [HttpPost]
        [Route("deliver")]
        public async Task<IHttpActionResult> DeliverInstrumentWithDataAsync([FromUri] string serverParkName, [FromUri] string instrumentName,
            [FromBody] DeliverInstrumentDto deliverInstrumentDto)
        {
            _loggingService.LogInfo($"Attempting to deliver instrument '{instrumentName}' on server park '{serverParkName}'");

            var instrumentPath = await _instrumentDataService.DeliverInstrumentPackageWithDataAsync(serverParkName, instrumentName, deliverInstrumentDto);

            _loggingService.LogInfo($"Instrument '{instrumentName}' delivered with data to '{deliverInstrumentDto.BucketPath}'");

            return Created($@"gs://{deliverInstrumentDto.BucketPath}", deliverInstrumentDto);
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> DownloadInstrumentWithDataAsync([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            _loggingService.LogInfo($"Attempting to download instrument '{instrumentName}' on server park '{serverParkName}'");

            var instrumentFile = await _instrumentDataService.DownloadInstrumentPackageWithDataAsync(serverParkName, instrumentName);

            return DownloadFile(instrumentFile);
        }
    }
}
