using System;
using System.Configuration;

namespace Blaise.Api.Providers
{
    public class ConfigurationProvider
    {
        public static string BaseUrl => Environment.GetEnvironmentVariable("ENV_BASE_URL", EnvironmentVariableTarget.Machine)
                                     ?? ConfigurationManager.AppSettings["BaseUrl"];
    }
}
