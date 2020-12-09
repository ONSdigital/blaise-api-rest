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
    public class ServerParkController : ApiController
    {
        private readonly IServerParkService _serverParkService;

        public ServerParkController(IServerParkService parkService)
        {
            _serverParkService = parkService;
        }

        [HttpGet]
        [Route("serverparks/names")]
        [ResponseType(typeof(IEnumerable<string>))]
        public IHttpActionResult GetServerParkNames()
        {
            try
            {
                Console.WriteLine("Obtaining a list of server parks");

                var serverParkNames = _serverParkService.GetServerParkNames().ToList();

                Console.WriteLine($"Successfully received a list of server park names '{string.Join(", ", serverParkNames)}'");

                return Ok(serverParkNames);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("serverparks")]
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
        [Route("serverparks/{serverParkName}")]
        [ResponseType(typeof(ServerParkDto))]
        public IHttpActionResult GetServerPark(string serverParkName)
        {
            try
            {
                var park = _serverParkService.GetServerPark(serverParkName);

                Console.WriteLine($"Successfully received server park '{serverParkName}'");

                return Ok(park);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ServerParkExists(string serverParkName)
        {
            try
            {
                var exists = _serverParkService.ServerParkExists(serverParkName);

                Console.WriteLine($"Successfully found server park '{serverParkName}'");

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
