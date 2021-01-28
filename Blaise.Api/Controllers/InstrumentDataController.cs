using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Logging.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/{instrumentName}/data")]
    public class InstrumentDataController : BaseController
    {
        private readonly IInstrumentDataService _instrumentDataService;

        public InstrumentDataController(IInstrumentDataService dataDeliveryService)
        {
            _instrumentDataService = dataDeliveryService;
        }

        [HttpPost]
        [Route("deliver")]
        public async Task<IHttpActionResult> DeliverInstrumentWithDataAsync([FromUri] string serverParkName, [FromUri] string instrumentName,
            [FromBody] DeliverInstrumentDto deliverInstrumentDto)
        {
            LoggingService.LogInfo($"Attempting to deliver instrument '{instrumentName}' on server park '{serverParkName}'");

            var instrumentPath = await _instrumentDataService.DeliverInstrumentPackageWithDataAsync(serverParkName, instrumentName, deliverInstrumentDto);

            LoggingService.LogInfo($"Instrument '{instrumentName}' delivered with data to '{deliverInstrumentDto.BucketPath}'");

            return Created($@"gs://{instrumentPath}", deliverInstrumentDto);
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> DownloadInstrumentWithDataAsync([FromUri] string serverParkName, [FromUri] string instrumentName)
        {
            LoggingService.LogInfo($"Attempting to download instrument '{instrumentName}' on server park '{serverParkName}'");

            var instrumentFile = await _instrumentDataService.DownloadInstrumentPackageWithDataAsync(serverParkName, instrumentName);

            return DownloadFile(instrumentFile);
        }
    }
}
