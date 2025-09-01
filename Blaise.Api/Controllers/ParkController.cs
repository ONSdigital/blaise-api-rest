using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Core.Interfaces;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1")]
    public class ParkController : ApiController
    {
        private readonly IParkService _parkService;

        public ParkController(IParkService parkService)
        {
            _parkService = parkService;
        }

        [HttpGet]
        [Route("parks")]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetParks()
        {
            try
            {
                Console.WriteLine("Obtaining a list of server parks");

                var parks = _parkService.GetParks().ToList();

                Console.WriteLine($"Successfully received a list of server parks '{string.Join(", ", parks)}'");

                return Ok(parks);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }
    }
}
