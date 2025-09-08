namespace Blaise.Api
{
    using System.Net.Http.Extensions.Compression.Core.Compressors;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using Blaise.Api.Configuration;
    using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
    using Microsoft.Owin.Extensions;
    using Newtonsoft.Json.Serialization;
    using Owin;
    using Unity;
    using Unity.WebApi;

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

            var config = new HttpConfiguration
            {
                DependencyResolver = UnityConfig.GetDependencyResolver(),
            };

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            appBuilder.UseWebApi(config);

            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));
            SwaggerConfig.Register(config);
        }
    }
}
