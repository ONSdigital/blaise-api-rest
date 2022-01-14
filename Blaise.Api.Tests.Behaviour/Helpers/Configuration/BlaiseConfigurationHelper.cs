using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class BlaiseConfigurationHelper
    {
        public static bool UseStubs => StubConfigurationHelper.UseStubs;
        public static string ServerParkName => UseStubs ? "gustyTest" : ConfigurationExtensions.GetEnvironmentVariable("ServerParkName");
        public static string InstrumentName => UseStubs ? "opnTest" : ConfigurationExtensions.GetEnvironmentVariable("InstrumentName");
        public static string InstrumentFile => UseStubs ? "instrumentFileTest.bkpg" : $"{InstrumentName}.{InstrumentExtension}";
        public static string InstrumentPath => ConfigurationExtensions.GetEnvironmentVariable("InstrumentPath");
        public static string InstrumentPackage => $"{InstrumentName}.{InstrumentExtension}";
        public static string InstrumentExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");
        public static string InstrumentPackagePath => $"{InstrumentPath}//{InstrumentPackage}";
        public static string InstrumentPackageBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_DQS_BUCKET");
        public static string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");
        public static string OnlineFileBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_NISRA_BUCKET");
    }
}
