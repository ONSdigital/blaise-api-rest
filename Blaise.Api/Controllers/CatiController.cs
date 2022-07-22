using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v2/cati")]
    public class CatiController : BaseController
    {
        private readonly ICatiService _catiService;
        private readonly ILoggingService _loggingService;

        public CatiController(
            ICatiService catiService, 
            ILoggingService loggingService)
        {
            _catiService = catiService;
            _loggingService = loggingService;
        }

        [HttpGet]
        [Route("questionnaires")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<CatiQuestionnaireDto>))]
        public IHttpActionResult GetQuestionnaires()
        {
            _loggingService.LogInfo("Obtaining a list of questionnaires from Cati");

            var questionnaires = _catiService.GetCatiQuestionnaires().ToList();

            _loggingService.LogInfo($"Successfully received {questionnaires.Count} questionnaires from Cati");

            return Ok(questionnaires);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/questionnaires")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(IEnumerable<CatiQuestionnaireDto>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaires([FromUri] string serverParkName)
        {
            _loggingService.LogInfo($"Obtaining a list of questionnaires from Cati for server park '{serverParkName}'");

            var questionnaires = _catiService.GetCatiQuestionnaires(serverParkName).ToList();

            _loggingService.LogInfo($"Successfully received {questionnaires.Count} questionnaires from Cati");

            return Ok(questionnaires);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(CatiQuestionnaireDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetQuestionnaire([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Obtaining a questionnaire from Cati for server park '{serverParkName}'");

            var questionnaires = _catiService.GetCatiQuestionnaire(serverParkName, questionnaireName);

            _loggingService.LogInfo("Successfully received a questionnaire from Cati");

            return Ok(questionnaires);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/daybatch")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DayBatchDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetDayBatch([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get a daybatch for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var dayBatchDto = _catiService.GetDayBatch(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Daybatch retrieved for questionnaire '{questionnaireName}' for '{dayBatchDto.DayBatchDate}'");

            return Ok(dayBatchDto);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/daybatch")]
        [SwaggerResponse(HttpStatusCode.Created, Type= typeof(DayBatchDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult CreateDayBatch([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromBody] CreateDayBatchDto createDayBatchDto)
        {
            _loggingService.LogInfo($"Create a daybatch for questionnaire '{questionnaireName}' on server park '{serverParkName}' for '{createDayBatchDto.DayBatchDate}'");

            var dayBatchDto = _catiService.CreateDayBatch(questionnaireName, serverParkName, createDayBatchDto);

            _loggingService.LogInfo($"Daybatch created for questionnaire '{questionnaireName}' on '{createDayBatchDto.DayBatchDate}'");

            return Created($"{Request.RequestUri}", dayBatchDto);
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/daybatch/today")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult HasDayBatchForToday([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Check a daybatch exists today for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var exists = _catiService.QuestionnaireHasADayBatchForToday(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Daybatch exists today for questionnaire'{questionnaireName}' - '{exists}'");

            return Ok(exists);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/daybatch/add")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult AddCasesToDayBatch([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromBody] List<string> caseIds)
        {
            _loggingService.LogInfo($"Add cases to the current daybatch for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            _catiService.AddCasesToDayBatch(questionnaireName, serverParkName, caseIds);

            _loggingService.LogInfo($"Cases added to daybatch to the current daybatch for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            return NoContent();
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<DateTime>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult GetSurveyDays([FromUri] string serverParkName, [FromUri] string questionnaireName)
        {
            _loggingService.LogInfo($"Get survey days for questionnaire '{questionnaireName}' on server park '{serverParkName}'");

            var surveyDays = _catiService.GetSurveyDays(questionnaireName, serverParkName);

            _loggingService.LogInfo($"Survey days retrieved for questionnaire '{questionnaireName}'");

            return Ok(surveyDays);
        }

        [HttpPost]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(List<DateTime>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult AddSurveyDays([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromBody] List<DateTime> surveyDays)
        {
            _loggingService.LogInfo($"Add survey days for questionnaire '{questionnaireName}' on server park '{serverParkName}' for '{surveyDays}'");

            surveyDays = _catiService.AddSurveyDays(questionnaireName, serverParkName, surveyDays);

            _loggingService.LogInfo($"Survey days added for questionnaire '{questionnaireName}'");

            return Created($"{Request.RequestUri}", surveyDays);
        }

        [HttpDelete]
        [Route("serverparks/{serverParkName}/questionnaires/{questionnaireName}/surveydays")]
        [SwaggerResponse(HttpStatusCode.NoContent, Type = null)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = null)]
        [SwaggerResponse(HttpStatusCode.NotFound, Type = null)]
        public IHttpActionResult RemoveSurveyDays([FromUri] string serverParkName, [FromUri] string questionnaireName, [FromBody] List<DateTime> surveyDays)
        {
            _loggingService.LogInfo($"Remove survey days for questionnaire '{questionnaireName}' on server park '{serverParkName}' for '{surveyDays}'");

             _catiService.RemoveSurveyDays(questionnaireName, serverParkName, surveyDays);

            _loggingService.LogInfo($"Survey days Removed for questionnaire '{questionnaireName}'");

            return NoContent();
        }
    }
}
