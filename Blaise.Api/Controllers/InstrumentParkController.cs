using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Blaise.Api.Contracts.Models;
using Blaise.Api.Core.Interfaces;
using Blaise.Nuget.Api.Contracts.Exceptions;

namespace Blaise.Api.Controllers
{
    [RoutePrefix("api/v1")]
    public class InstrumentParkController : ApiController
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentParkController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        [HttpGet]
        [Route("instruments")]
        [ResponseType(typeof(IEnumerable<InstrumentDto>))]
        public IHttpActionResult GetAllInstruments()
        {
            try
            {
                Console.WriteLine("Obtaining a list of instruments across all server parks");

                var instruments = _instrumentService.GetAllInstruments().ToList();

                Console.WriteLine($"Successfully received a list of instruments '{string.Join(", ", instruments)}'");

                return Ok(instruments);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments")]
        [ResponseType(typeof(IEnumerable<InstrumentDto>))]
        public IHttpActionResult GetInstruments(string serverParkName)
        {
            try
            {
                Console.WriteLine("Obtaining a list of instruments for a server park");

                var instruments = _instrumentService.GetInstruments(serverParkName).ToList();

                Console.WriteLine($"Successfully received a list of instruments '{string.Join(", ", instruments)}'");

                return Ok(instruments);
            }
            catch (DataNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}")]
        [ResponseType(typeof(InstrumentDto))]
        public IHttpActionResult GetInstrument(string serverParkName, string instrumentName)
        {
            try
            {
                Console.WriteLine("Obtaining an instruments for a server park");

                var instruments = _instrumentService
                    .GetInstrument(instrumentName, serverParkName);

                Console.WriteLine($"Successfully received an instrument '{instrumentName}'");

                return Ok(instruments);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}, {e.InnerException}");
                throw;
            }
        }

        [HttpGet]
        [Route("serverparks/{serverParkName}/instruments/{instrumentName}/exists")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult InstrumentExists(string serverParkName, string instrumentName)
        {
            try
            {
                Console.WriteLine("Check that an instrument exists on a server park");

                var exists = _instrumentService
                    .InstrumentExists(instrumentName, serverParkName);

                Console.WriteLine($"Instrument '{instrumentName}' exists = {exists}");

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
