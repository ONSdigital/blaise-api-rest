using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Extensions;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/{instrumentName}/data")]
    public class InstrumentDataController : BaseController
    {
        private readonly IInstrumentDataService _instrumentDataService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public InstrumentDataController(
            IInstrumentDataService dataDeliveryService, 
            ILoggingService loggingService, 
            IConfigurationProvider configurationProvider)
        {
            _instrumentDataService = dataDeliveryService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(InstrumentDataDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public async Task<IHttpActionResult> PostInstrumentWithDataAsync([FromUri] string serverParkName, 
            [FromUri] string instrumentName, [FromBody] InstrumentDataDto instrumentDataDto)
        {
            var tempPath = _configurationProvider.TempPath;
            
            try
            {
                _loggingService.LogInfo($"Attempting to import instrument '{instrumentName}' with data on server park '{serverParkName}'");
                await _instrumentDataService.ImportOnlineDataAsync(instrumentDataDto, serverParkName, instrumentName, tempPath);
                return Created($"{Request.RequestUri}", instrumentDataDto);
            }
            finally
            {
                tempPath.CleanUpTempFiles();
                _loggingService.LogInfo($"Removed temporary files and folder '{tempPath}'");
            }
        }
    }
}
