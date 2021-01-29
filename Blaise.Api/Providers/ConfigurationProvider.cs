using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Extensions;

namespace Blaise.Api.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string BaseUrl => ConfigurationExtensions.GetVariable("BASE_URL");

        public string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");
        public string BucketPath => ConfigurationExtensions.GetEnvironmentVariable("ENV_BLAISE_DQS_BUCKET");
        public string PackageExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");
    }
}
