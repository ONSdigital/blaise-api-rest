using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/serverparks/{serverParkName}/instruments/{instrumentName}/cases")]
    public class CaseController : BaseController
    {
        private readonly ICaseService _caseService;

        public CaseController(
            ICaseService caseService,
            ILoggingService loggingService) : base(loggingService)
        {
            _caseService = caseService;
        }

        [HttpGet]
        [Route("ids")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseIds([FromUri] string instrumentName)
        {
            var caseIds = _caseService.GetCaseIds(instrumentName);

            return Ok(caseIds);
        }

        [HttpGet]
        [Route("identifiers")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CaseIdentifierDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseIdentifiers([FromUri] string instrumentName)
        {
            var caseIdentifiers = _caseService.GetCaseIdentifiers(instrumentName);

            return Ok(caseIdentifiers);
        }

        [HttpGet]
        [Route("{caseId}/postcode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseIds([FromUri] string serverParkName, [FromUri] string instrumentName, [FromUri] string caseId)
        {
            var postCode = _caseService.GetPostCode(serverParkName, instrumentName, caseId);

            return Ok(postCode);
        }
    }
}
