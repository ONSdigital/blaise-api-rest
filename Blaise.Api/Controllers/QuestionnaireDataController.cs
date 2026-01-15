namespace Blaise.Api.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Blaise.Api.Configuration;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Interfaces.Services;
    using Swashbuckle.Swagger.Annotations;

    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires/{questionnaireName}/data")]
    public class QuestionnaireDataController : BaseController
    {
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;

        public QuestionnaireDataController(
            ILoggingService loggingService,
            IConfigurationProvider configurationProvider)
        {
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Accepted, Type = typeof(QuestionnaireDataDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult PostQuestionnaireWithData(
            [FromUri] string serverParkName,
            [FromUri] string questionnaireName,
            [FromBody] QuestionnaireDataDto questionnaireDataDto)
        {
            var questionnaireService = UnityConfig.Resolve<IQuestionnaireService>();
            if (!questionnaireService.QuestionnaireExists(questionnaireName, serverParkName))
            {
                _loggingService.LogWarn($"Request to import data for unknown questionnaire '{questionnaireName}'");
                return NotFound();
            }

            var tempPath = _configurationProvider.TempPath;
            _loggingService.LogInfo($"Processing data import for questionnaire '{questionnaireName}'");

            Task.Run(async () =>
            {
                try
                {
                    var backgroundDataService = UnityConfig.Resolve<IQuestionnaireDataService>();
                    await backgroundDataService.ImportOnlineDataAsync(questionnaireDataDto, serverParkName, questionnaireName, tempPath);
                    UnityConfig.Resolve<ILoggingService>().LogInfo($"Data import for questionnaire '{questionnaireName}' complete");
                }
                catch (Exception ex)
                {
                    UnityConfig.Resolve<ILoggingService>().LogError($"Data import for questionnaire '{questionnaireName}' failed", ex);
                }
            });

            return Content(HttpStatusCode.Accepted, questionnaireDataDto);
        }
    }
}
