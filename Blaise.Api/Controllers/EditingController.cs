using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/editing")]
    public class EditingController : BaseController
    {
        private readonly IEditingService _editingService;
        private readonly ILoggingService _loggingService;

        public EditingController(
            IEditingService editingService,
            ILoggingService loggingService)
        {
            _editingService = editingService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("caseEditingDetails")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetCaseEditingDetailsList([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            var caseEditingDetails = _editingService.GetCaseEditingDetailsList(serverParkName, questionnaireName);

            _loggingService.LogInfo($"Successfully got case editing details list");

            return Ok(caseEditingDetails);
        }
    }
}
