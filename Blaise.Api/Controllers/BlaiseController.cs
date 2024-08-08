using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/profession")]
    public class BlaiseController : BaseController
    {

        private readonly List<string> _professions;

        public BlaiseController()
        {
            _professions = new List<string>
            {
                "Business Analyst",
                "Business Program Manager",
                "Business Product Owner",
                "IT Consultant",
                "IT Programmer",
                "IT Support"                
            };
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult GetProfessionOptions([FromBody] string searchData)
        {
            var professionOptions = _professions.Where(p => p.IndexOf(searchData, StringComparison.OrdinalIgnoreCase) >= 0);

            return Ok(string.Join(",", professionOptions));
        }
    }
}