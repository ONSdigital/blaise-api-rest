using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Extensions;

namespace Blaise.Api.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string BaseUrl => ConfigurationExtensions.GetVariable("BASE_URL");
        public string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");
        public string PackageExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");
        public string DqsBucket => ConfigurationExtensions.GetEnvironmentVariable("ENV_BLAISE_DQS_BUCKET");
    }
}
