using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/questionnaires")]
    public class ReportingV2Controller : BaseController
    {
        private readonly IReportingServiceV2 _reportingService;

        public ReportingV2Controller(IReportingServiceV2 reportingService)
        {
            _reportingService = reportingService;
        }

        [HttpGet]
        [Route("{questionnaireName}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportV2Dto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] string questionnaireName,
            [FromUri] List<string> fieldIds)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, questionnaireName, fieldIds);

            return Ok(reportDto);
        }

        [HttpGet]
        [Route("{questionnaireId:guid}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportV2Dto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] Guid questionnaireId,
            [FromUri] List<string> fieldIds)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, questionnaireId, fieldIds);

            return Ok(reportDto);
        }
    }
}
