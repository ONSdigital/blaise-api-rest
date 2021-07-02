using System.Net;
using System.Web.Http;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Swashbuckle.Swagger.Annotations;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1/jake")]
    public class JakeController : BaseController
    {
        private readonly IJakeService _jakeService;
        public JakeController(ILoggingService loggingService, IJakeService jakeService) : base(loggingService)
        {
            _jakeService = jakeService;
        }

        [HttpGet]
        [Route("greet")]
        [SwaggerResponse(HttpStatusCode.OK, Type=typeof(string))]
        public IHttpActionResult GreetCustomer([FromUri] string greeting = null, [FromUri] string name = null)
        {

            var response = _jakeService.GreetCustomer(greeting, name);

            return Ok(response);
        }
    }
}
