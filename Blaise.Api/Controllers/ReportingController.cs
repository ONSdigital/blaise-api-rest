using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires")]
    public class ReportingController : BaseController
    {
        private readonly IReportingService _reportingService;
        private readonly ILoggingService _loggingService;

        public ReportingController(
            IReportingService reportingService,
            ILoggingService loggingService)
        {
            _reportingService = reportingService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("{questionnaireName}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] string questionnaireName,
            [FromUri] List<string> fieldIds, [FromUri] string filter = null)
        {
            _loggingService.LogInfo($"Obtaining the fields '{string.Join(",", fieldIds)}' with filter '{filter ?? string.Empty}' for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var reportDto = _reportingService.GetReportingData(serverParkName, questionnaireName, fieldIds, filter);

            _loggingService.LogInfo($"Obtained the fields for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            return Ok(reportDto);
        }

        [HttpGet]
        [Route("{questionnaireId:guid}/report")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetReportingData([FromUri] string serverParkName, [FromUri] Guid questionnaireId,
            [FromUri] List<string> fieldIds, [FromUri] string filter = null)
        {
            var reportDto = _reportingService.GetReportingData(serverParkName, questionnaireId, fieldIds, filter);

            return Ok(reportDto);
        }
    }
}
