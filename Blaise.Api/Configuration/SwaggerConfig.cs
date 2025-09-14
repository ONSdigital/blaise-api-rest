using System.Configuration;
using System.Web.Http;
using Blaise.Api.Configuration;
using Swashbuckle.Application;
using WebActivatorEx;

namespace Blaise.Api.Configuration
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            string description = "Blaise.Api";

            var hostName = ConfigurationManager.AppSettings["ENV_BLAISE_SERVER_HOST_NAME"];

            if (!string.IsNullOrEmpty(hostName) && hostName.ToLower().Contains("ons-blaise-v2-prod"))
            {
                description = "⚠️ **DANGER: This is the PRODUCTION environment!** ⚠️ <br>" +
                              "Ensure you are pairing with another developer!";
            }

            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Blaise.Api").Description(description);
            }).EnableSwaggerUi(c =>
            {
            });
        }
    }
}
