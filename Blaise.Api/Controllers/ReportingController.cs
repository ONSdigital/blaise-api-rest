using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/{instrumentName}/report")]
    public class ReportingController : BaseController
    {
        private readonly IReportingService _reportingService;
        private readonly ILoggingService _loggingService;

        public ReportingController(
            IReportingService reportingService,
            ILoggingService loggingService) : base(loggingService)
        {
            _reportingService = reportingService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] string instrumentName,
            [FromUri] string[] fieldIds)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, instrumentName, fieldIds);

            return Ok(reportDto);

        }
    }
}
