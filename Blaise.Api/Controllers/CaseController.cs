using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Case;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Blaise.Api.Controllers
{
    using StatNeth.Blaise.API.Meta.Constants;
    using System.Linq;
    using System.Web.Http.Description;

    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/cases")]
    public class CaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly ILoggingService _loggingService;

        public CaseController(
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
        [Route("{caseId}/exists")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseExists([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string caseId)
        {
            var exists = _caseService.CaseExists(serverParkName, questionnaireName, caseId);

            return Ok(exists);
        }


        [HttpGet]
        [Route("/multikey")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseExists([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] List<string> keyNames, [FromUri] List<string> keyValues)
        {
            var exists = _caseService.CaseExists(serverParkName, questionnaireName, keyNames, keyValues);

            return Ok(exists);
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

        [HttpGet]
        [Route("/multikey")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CaseDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] List<string> keyNames, [FromUri] List<string> keyValues)
        {
            // _loggingService.LogInfo($"Attempting to get case '{caseId}'");

            var caseDto = _caseService.GetCase(serverParkName, questionnaireName, keyNames, keyValues);

           // _loggingService.LogInfo($"Successfully got case '{caseId}'");

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

        [HttpPost]
        [Route("/multikey")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] List<string> keyNames, [FromUri] List<string> keyValues,
            [FromBody] Dictionary<string, string> fieldData)
        {
            //_loggingService.LogInfo($"Attempting to create case '{caseId}'");

            _caseService.CreateCase(serverParkName, questionnaireName, keyNames, keyValues, fieldData);

            // _loggingService.LogInfo($"Successfully created case '{caseId}'");
            var urlParams = "";
            return Created($"{Request.RequestUri}/cases/multikey?{urlParams}", urlParams);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateCases([FromUri] string serverParkName, [FromUri] string questionnaireName,
                                             [FromBody] List<CaseDto> caseData)
        {
            if (caseData == null || !caseData.Any())
            {
                _loggingService.LogError("Error - Invalid JSON", null);
                return BadRequest();
            }

            _loggingService.LogInfo($"Attempting to create cases ({caseData.Count}) for server park {serverParkName} and questionnaire {questionnaireName}");

            if (_caseService.CreateCases(caseData, questionnaireName, serverParkName) == 0)
            {
                _loggingService.LogInfo($"Error creating cases for server park {serverParkName} and questionnaire {questionnaireName}");
                return BadRequest();
            }

            _loggingService.LogInfo($"Successfully created cases for server park {serverParkName} and questionnaire {questionnaireName}");
            return Created("", "");
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

        [HttpPatch]
        [Route("/multikey")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UpdateCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] List<string> keyNames, [FromUri] List<string> keyValues,
            [FromBody] Dictionary<string, string> fieldData)
        {
            //_loggingService.LogInfo($"Attempting to update case '{caseId}'");

            _caseService.UpdateCase(serverParkName, questionnaireName, keyNames, keyValues, fieldData);

            //_loggingService.LogInfo($"Successfully updated case '{caseId}'");

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("/multikey")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DeleteCase([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] List<string> keyNames, [FromUri] List<string> keyValues,)
        {
            //_loggingService.LogInfo($"Attempting to delete case '{caseId}'");

            _caseService.DeleteCase(serverParkName, questionnaireName, keyNames, keyValues);

            //_loggingService.LogInfo($"Successfully deleted case '{caseId}'");

            return NoContent();
        }
    }
}
