﻿using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Filters;

namespace Blaise.Api.Controllers
{
    [ExceptionFilter]
    public class BaseController : ApiController
    {
        private readonly ILoggingService _loggingService;


        public BaseController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        internal StatusCodeResult NoContent()
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        internal IHttpActionResult DownloadFile(string filePath)
        {
            _loggingService.LogInfo($"Downloading file '{filePath}'");
            var responseMsg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(File.ReadAllBytes(filePath))
            };

            responseMsg.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = Path.GetFileName(filePath) };
            responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(responseMsg);
        }
    }
}
