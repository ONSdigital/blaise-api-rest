using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments")]
    public class ReportingController : BaseController
    {
        private readonly IReportingService _reportingService;

        public ReportingController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet]
        [Route("{instrumentName}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] string instrumentName,
            [FromUri] List<string> fieldIds)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, instrumentName, fieldIds);

            return Ok(reportDto);
        }

        [HttpGet]
        [Route("{instrumentId:guid}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] Guid instrumentId,
            [FromUri] List<string> fieldIds)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, instrumentId, fieldIds);

            return Ok(reportDto);
        }
    }
}
