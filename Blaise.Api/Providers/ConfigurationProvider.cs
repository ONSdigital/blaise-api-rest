namespace Blaise.Api.Providers
{
    using System;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Extensions;

    public class ConfigurationProvider : IConfigurationProvider
    {
        public string BaseUrl => $"{ConfigurationExtensions.GetVariable("BASE_URL")}{ConfigurationExtensions.GetVariable("ENV_RESTAPI_PORT")}/";

        public string TempPath => $"{ConfigurationExtensions.GetVariable("TEMP_PATH")}\\{Guid.NewGuid()}";

        public string PackageExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");

        public string DqsBucket => ConfigurationExtensions.GetEnvironmentVariable("ENV_BLAISE_DQS_BUCKET");

        public string NisraBucket => ConfigurationExtensions.GetEnvironmentVariable("ENV_BLAISE_NISRA_BUCKET");

        public string IngestBucket => ConfigurationExtensions.GetEnvironmentVariable("ENV_BLAISE_INGEST_BUCKET");
    }
}
