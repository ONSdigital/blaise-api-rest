using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/ingest")]
    public class IngestController : BaseController
    {
        private readonly IIngestService _ingestService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public IngestController(
            IIngestService ingestService,
            ILoggingService loggingService,
            IConfigurationProvider configurationProvider)
        {
            _ingestService = ingestService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(QuestionnaireDataDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public async Task<IHttpActionResult> PostIngestQuestionnaireAsync([FromUri] string serverParkName,
            [FromUri] string questionnaireName, [FromBody] QuestionnaireDataDto questionnaireDataDto)
        {
            var tempPath = _configurationProvider.TempPath;
            
            _loggingService.LogInfo(
                $"Attempting to ingest questionnaire '{questionnaireName}' data on server park '{serverParkName}'");
            await _ingestService.IngestDataAsync(questionnaireDataDto, serverParkName, questionnaireName, tempPath);
            return Created($"{Request.RequestUri}", questionnaireDataDto);
        }
    }
}