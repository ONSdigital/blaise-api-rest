namespace Blaise.Api.Controllers
{
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Blaise.Api.Filters;

    [ExceptionFilter]
    public class BaseController : ApiController
    {
        internal StatusCodeResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
