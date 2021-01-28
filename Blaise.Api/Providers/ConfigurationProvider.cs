using System.Configuration;
using Blaise.Api.Contracts.Interfaces;

namespace Blaise.Api.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string BaseUrl => ConfigurationManager.AppSettings["BASE_URL"];

        public string TempPath => ConfigurationManager.AppSettings["TEMP_PATH"];
        public string BucketPath => ConfigurationManager.AppSettings["ENV_BLAISE_GCP_BUCKET"];
        public string PackageExtension => ConfigurationManager.AppSettings["ENV_PACKAGE_EXTENSION"];
    }
}
