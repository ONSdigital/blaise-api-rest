using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Blaise.Api.Filters;

namespace Blaise.Api.Controllers
{
    [ExceptionFilter]
    public class BaseController : ApiController
    {
        internal StatusCodeResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
