using System.Web.Http;
using Blaise.Api.Configuration;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Blaise.Api.Configuration
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Blaise.Api")
                        .Description("**Caution: Ensure that you are pairing when working in the PRODUCTION environment.**");
                })
                .EnableSwaggerUi(c =>
                {
                });
        }
    }
}
