namespace Blaise.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Nuget.Api.Contracts.Enums;
    using Swashbuckle.Swagger.Annotations;

    [RoutePrefix("api/v2/serverparks/{serverParkName}/questionnaires")]
    public class QuestionnaireController : BaseController
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IQuestionnaireInstallerService _questionnaireInstallerService;
        private readonly IQuestionnaireUninstallerService _questionnaireUninstallerService;
        private readonly ILoggingService _loggingService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRetryService<Exception> _retryService;

        public QuestionnaireController(
            IQuestionnaireService questionnaireService,
            IQuestionnaireInstallerService questionnaireInstallerService,
            IQuestionnaireUninstallerService questionnaireUninstallerService,
            ILoggingService loggingService,
            IConfigurationProvider configurationProvider,
            IRetryService<Exception> retryService)
        {
            _questionnaireService = questionnaireService;
            _questionnaireInstallerService = questionnaireInstallerService;
            _questionnaireUninstallerService = questionnaireUninstallerService;
            _loggingService = loggingService;
            _configurationProvider = configurationProvider;
            _retryService = retryService;
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<QuestionnaireDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaires(string serverParkName)
        {
            _loggingService.LogInfo("Obtaining a list of questionnaires for a server park");

            var questionnaires = _retryService.Retry(_questionnaireService.GetQuestionnaires, serverParkName).ToList();

            _loggingService.LogInfo($"Successfully received {questionnaires.Count} questionnaires");

            return Ok(questionnaires);
        }

        [HttpGet]
        [Route("{questionnaireName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(QuestionnaireDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaire([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo("Get a questionnaire for a server park");

            var questionnaires = _questionnaireService
                .GetQuestionnaire(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Successfully retrieved a questionnaire '{questionnaireName}'");

            return Ok(questionnaires);
        }

        [HttpGet]
        [Route("{questionnaireName}/exists")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult QuestionnaireExists([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Check that a questionnaire exists on server park '{serverParkName}'");

            var exists = _questionnaireService.QuestionnaireExists(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Questionnaire '{questionnaireName}' exists = '{exists}' on '{serverParkName}'");

            return Ok(exists);
        }

        [HttpGet]
        [Route("{questionnaireName}/id")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Guid))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaireId([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get the ID of a questionnaire on server park '{serverParkName}'");

            var questionnaireId = _questionnaireService.GetQuestionnaireId(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Questionnaire ID '{questionnaireId}' retrieved");

            return Ok(questionnaireId);
        }

        [HttpGet]
        [Route("{questionnaireName}/status")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(QuestionnaireStatusType))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaireStatus([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get the status of a questionnaire on server park '{serverParkName}'");

            var status = _questionnaireService.GetQuestionnaireStatus(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Questionnaire'{questionnaireName}' has the status '{status}'");

            return Ok(status);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(QuestionnairePackageDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public async Task<IHttpActionResult> InstallQuestionnaire([FromUri] string serverParkName, [FromBody] QuestionnairePackageDto questionnairePackageDto)
        {
            var tempPath = _configurationProvider.TempPath;

            _loggingService.LogInfo($"Attempting to install questionnaire '{questionnairePackageDto.QuestionnaireFile}' on server park '{serverParkName}'");

            var questionnaireName = await _questionnaireInstallerService.InstallQuestionnaireAsync(serverParkName, questionnairePackageDto, tempPath);

            _loggingService.LogInfo($"Questionnaire '{questionnairePackageDto.QuestionnaireFile}' installed on server park '{serverParkName}'");

            return Created($"{Request.RequestUri}/{questionnaireName}", questionnairePackageDto);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("{questionnaireName}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult UninstallQuestionnaire([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Attempting to uninstall questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            _questionnaireUninstallerService.UninstallQuestionnaire(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Questionnaire '{questionnaireName}' has been uninstalled from server park '{serverParkName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{questionnaireName}/activate")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult ActivateQuestionnaire([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Activate questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            _questionnaireService
                .ActivateQuestionnaire(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Successfully activated questionnaire '{questionnaireName}'");

            return NoContent();
        }

        [HttpPatch]
        [Route("{questionnaireName}/deactivate")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DeactivateQuestionnaire([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Deactivate questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            _questionnaireService
                .DeactivateQuestionnaire(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Successfully deactivated questionnaire '{questionnaireName}'");

            return NoContent();
        }

        [HttpGet]
        [Route("{questionnaireName}/modes")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<string>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetModes([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get modes for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var modes = _questionnaireService.GetModes(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Got modes for questionnaire '{questionnaireName}': '{modes}'");

            return Ok(modes);
        }

        [HttpGet]
        [Route("{questionnaireName}/modes/{mode}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult ModeExists([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromUri] string mode)
        {
            _loggingService.LogInfo($"Check if a mode exists for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var exists = _questionnaireService.ModeExists(questionnaireName, serverParkName, mode);

            _loggingService.LogInfo($"Mode exists for questionnaire '{questionnaireName}': '{exists}'");

            return Ok(exists);
        }

        [HttpGet]
        [Route("{questionnaireName}/settings")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DataEntrySettingsDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult DataEntrySettings([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get settings for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var dataEntrySettings = _questionnaireService.GetDataEntrySettings(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Got settings for questionnaire '{questionnaireName}': '{dataEntrySettings}'");

            return Ok(dataEntrySettings);
        }
    }
}
