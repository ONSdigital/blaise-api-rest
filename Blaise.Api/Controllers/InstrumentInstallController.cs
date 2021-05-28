using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Extensions;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}")]
    public class InstrumentInstallController : BaseController
    {
        private readonly IInstrumentInstallerService _installInstrumentService;
        private readonly IInstrumentUninstallerService _uninstallInstrumentService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public InstrumentInstallController(
            IInstrumentInstallerService installInstrumentService,
            IInstrumentUninstallerService uninstallInstrumentService,
            ILoggingService loggingService, 
            IConfigurationProvider configurationProvider) : base(loggingService)
        {
            _installInstrumentService = installInstrumentService;
            _uninstallInstrumentService = uninstallInstrumentService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }
        
        [HttpPost]
        [Route("instruments")]
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
        [Route("instruments/{instrumentName}")]
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
    }
}
