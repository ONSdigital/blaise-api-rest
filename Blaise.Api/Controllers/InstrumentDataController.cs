using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Logging.Services;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/data")]
    public class InstrumentDataController : BaseController
    {
        private readonly IInstrumentDataService _deliverInstrumentService;

        public InstrumentDataController(IInstrumentDataService dataDeliveryService)
        {
            _deliverInstrumentService = dataDeliveryService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> DeliverInstrumentWithDataAsync([FromUri] string serverParkName, [FromBody] InstrumentPackageDto instrumentPackageDto)
        {
            LoggingService.LogInfo($"Attempting to deliver instrument '{instrumentPackageDto.InstrumentFile}' on server park '{serverParkName}'");

            var bucketPath = await _deliverInstrumentService.DeliverInstrumentPackageWithDataAsync(serverParkName, instrumentPackageDto);

            LoggingService.LogInfo($"Instrument '{instrumentPackageDto.InstrumentFile}' delivered with data");

            return Created($@"gs://{bucketPath}", instrumentPackageDto);
        }

        [HttpPost]
        [Route("download")]
        public async Task<IHttpActionResult> DownloadInstrumentWithDataAsync([FromUri] string serverParkName, [FromBody] InstrumentPackageDto instrumentPackageDto)
        {
            LoggingService.LogInfo($"Attempting to download instrument '{instrumentPackageDto.InstrumentFile}' on server park '{serverParkName}'");

            var instrumentFile = await _deliverInstrumentService.DeliverInstrumentPackageWithDataAsync(serverParkName, instrumentPackageDto);

            return DownloadFile(instrumentFile);
        }
    }
}
