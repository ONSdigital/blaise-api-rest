using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Logging.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments")]
    public class InstallInstrumentController : BaseController
    {
        private readonly IInstrumentInstallerService _installInstrumentService;
        private readonly IInstrumentUninstallerService _uninstallInstrumentService;
        private readonly IBlaiseFileService _fileService;

        public InstallInstrumentController(
            IInstrumentInstallerService installInstrumentService, 
            IInstrumentUninstallerService uninstallInstrumentService, 
            IBlaiseFileService fileService)
        {
            _installInstrumentService = installInstrumentService;
            _uninstallInstrumentService = uninstallInstrumentService;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> InstallInstrument([FromUri] string serverParkName, [FromBody] InstrumentPackageDto instrumentPackageDto)
        {
            LoggingService.LogInfo($"Attempting to install instrument '{instrumentPackageDto.InstrumentFile}' on server park '{serverParkName}'");

            await _installInstrumentService.InstallInstrumentAsync(serverParkName, instrumentPackageDto);

            LoggingService.LogInfo($"Instrument '{instrumentPackageDto.InstrumentFile}' installed on server park '{serverParkName}'");

            var instrumentName = _fileService.GetInstrumentNameFromFile(instrumentPackageDto.InstrumentFile);
            return Created($"{Request.RequestUri}/{instrumentName}", instrumentPackageDto);
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
    }
}
