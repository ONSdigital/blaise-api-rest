﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Blaise.Api.Configuration;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Exceptions;

namespace Blaise.Api.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILoggingService _loggingService;
        public ExceptionFilter()
        {
            _loggingService = UnityConfig.Resolve<ILoggingService>();
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            switch (context.Exception)
            {
                case DataNotFoundException _:
                    context.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    _loggingService.LogWarn($"Warning: {context.Exception}");
                    break;
                case ArgumentNullException _:
                case ArgumentException _:
                    context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    _loggingService.LogError("Error: ", context.Exception);
                    break;
                default:
                    context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    _loggingService.LogError("Error: ", context.Exception);
                    break;
            }
        }
    }
}
