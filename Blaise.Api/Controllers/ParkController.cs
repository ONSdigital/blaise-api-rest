using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1")]
    public class ParkController : ApiController
    {
        private readonly IServerParkService _serverParkService;

        public ParkController(IServerParkService parkService)
        {
            _serverParkService = parkService;
        }

        [HttpGet]
        [Route("parks/names")]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetServerParkNames()
        {
            try
            {
                Console.WriteLine("Obtaining a list of server parks");

                var parkNames = _serverParkService.GetServerParkNames().ToList();

                Console.WriteLine($"Successfully received a list of server park names '{string.Join(", ", parkNames)}'");

                return Ok(parkNames);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("parks")]
        [ResponseType(typeof(IEnumerable<ServerParkDto>))]
        public IHttpActionResult GetServerParks()
        {
            try
            {
                Console.WriteLine("Obtaining a list of server parks");

                var parks = _serverParkService.GetServerParks().ToList();

                Console.WriteLine($"Successfully received a list of server parks '{string.Join(", ", parks)}'");

                return Ok(parks);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("parks/{name}")]
        [ResponseType(typeof(ServerParkDto))]
        public IHttpActionResult GetServerPark(string name)
        {
            try
            {
                var park = _serverParkService.GetServerPark(name);

                Console.WriteLine($"Successfully received server park '{name}'");

                return Ok(park);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("parks/{name}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ServerParkExists(string name)
        {
            try
            {
                var exists = _serverParkService.ServerParkExists(name);

                Console.WriteLine($"Successfully found server park '{name}'");

                return Ok(exists);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }
    }
}
