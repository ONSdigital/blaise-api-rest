using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/data")]
    public class QuestionnaireDataController : BaseController
    {
        private readonly IQuestionnaireDataService _questionnaireDataService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public QuestionnaireDataController(
            IQuestionnaireDataService questionnaireDataService,
            ILoggingService loggingService,
            IConfigurationProvider configurationProvider)
        {
            _questionnaireDataService = questionnaireDataService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(QuestionnaireDataDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public async Task<IHttpActionResult> PostQuestionnaireWithDataAsync([FromUri] string serverParkName,
            [FromUri] string questionnaireName, [FromBody] QuestionnaireDataDto questionnaireDataDto)
        {
            var tempPath = _configurationProvider.TempPath;
            
            _loggingService.LogInfo(
                $"Attempting to import questionnaire '{questionnaireName}' with data on server park '{serverParkName}'");
            await _questionnaireDataService.ImportOnlineDataAsync(questionnaireDataDto, serverParkName, questionnaireName, tempPath);
            return Created($"{Request.RequestUri}", questionnaireDataDto);
        }
    }
}