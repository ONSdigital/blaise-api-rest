﻿using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/cases")]
    public class CaseV2Controller : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly ILoggingService _loggingService;

        public CaseV2Controller(
            ICaseService caseService,
            ILoggingService loggingService)
        {
            _caseService = caseService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("ids")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseIds([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            var caseIds = _caseService.GetCaseIds(serverParkName, questionnaireName);

            return Ok(caseIds);
        }

        [HttpGet]
        [Route("status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<CaseStatusDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetStatusOfCases([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            var caseStatusDtoList = _caseService.GetCaseStatusList(serverParkName, questionnaireName);

            return Ok(caseStatusDtoList);
        }

        [HttpGet]
        [Route("{caseId}/postcode")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseIds([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId)
        {
            var postCode = _caseService.GetPostCode(serverParkName, questionnaireName, caseId);

            return Ok(postCode);
        }

        [HttpGet]
        [Route("{caseId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CaseDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId)
        {
            _loggingService.LogInfo($"Attempting to get case '{caseId}'");

            var caseDto = _caseService.GetCase(serverParkName, questionnaireName, caseId);

            _loggingService.LogInfo($"Successfully got case '{caseId}'");

            return Ok(caseDto);
        }

        [HttpPost]
        [Route("{caseId}")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId, 
            [FromBody] Dictionary<string, string> fieldData)
        {
            _loggingService.LogInfo($"Attempting to create case '{caseId}'");

            _caseService.CreateCase(serverParkName, questionnaireName, caseId, fieldData);

            _loggingService.LogInfo($"Successfully created case '{caseId}'");

            return Created($"{Request.RequestUri}/{caseId}", caseId);
        }

        [HttpPatch]
        [Route("{caseId}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UpdateCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId,
            [FromBody] Dictionary<string, string> fieldData)
        {
            _loggingService.LogInfo($"Attempting to update case '{caseId}'");

            _caseService.UpdateCase(serverParkName, questionnaireName, caseId, fieldData);

            _loggingService.LogInfo($"Successfully updated case '{caseId}'");

            return NoContent();
        }

        [HttpDelete]
        [Route("{caseId}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DeleteCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId)
        {
            _loggingService.LogInfo($"Attempting to delete case '{caseId}'");

            _caseService.DeleteCase(serverParkName, questionnaireName, caseId);

            _loggingService.LogInfo($"Successfully deleted case '{caseId}'");

            return NoContent();
        }
    }
}