using System.Net.Http.Headers;
using System.Web.Http;
using Blaise.Api.Configuration;
using Microsoft.Owin.Extensions;
using Newtonsoft.Json.Serialization;
using Owin;
using Unity.WebApi;

namespace Blaise.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Use((context, next) =>
            {
                context.Response.Headers.Remove("Server");
                return next.Invoke();
            });
            appBuilder.UseStageMarker(PipelineStage.PostAcquireState);

            // Configure Web API for self-host. 
            var config = new HttpConfiguration
            {
                DependencyResolver = new UnityDependencyResolver(
                    UnityConfig.GetConfiguredContainer())
            };

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);

            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            SwaggerConfig.Register(config);
        }
    }
}
