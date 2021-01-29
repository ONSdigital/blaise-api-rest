using Blaise.Api.Tests.Helpers.Extensions;

namespace Blaise.Api.Tests.Helpers.Configuration
{
    public static class BlaiseConfigurationHelper
    {
        public static string ServerParkName => ConfigurationExtensions.GetEnvironmentVariable("ServerParkName");
        public static string InstrumentPath => ConfigurationExtensions.GetEnvironmentVariable("InstrumentPath");
        public static string InstrumentName => ConfigurationExtensions.GetEnvironmentVariable("InstrumentName");
        public static string InstrumentPackage => $"{InstrumentPath}//{InstrumentName}.zip";
        public static string InstrumentFile => $"{InstrumentName}.{ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION")}";
        public static string BucketName => ConfigurationExtensions.GetVariable("ENV_BLAISE_GCP_BUCKET");
        public static string InstrumentBucketPath => $"{BucketName}";
        public static string DeliveredInstrumentBucketPath => $"{BucketName}//data//{InstrumentName}";
        public static string TempDownloadPath => $@"c:\temp\instruments";
    }
}
