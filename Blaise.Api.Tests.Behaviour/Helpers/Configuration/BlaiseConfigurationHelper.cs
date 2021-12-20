﻿using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class BlaiseConfigurationHelper
    {
        public static string ServerParkName => ConfigurationExtensions.GetEnvironmentVariable("ServerParkName");
        public static string InstrumentPath => ConfigurationExtensions.GetEnvironmentVariable("InstrumentPath");
        public static string InstrumentName => ConfigurationExtensions.GetEnvironmentVariable("InstrumentName");
        public static string InstrumentExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");
        public static string InstrumentPackage => $"{InstrumentPath}//{InstrumentName}.{InstrumentExtension}";
        public static string InstrumentFile => $"{InstrumentName}.{InstrumentExtension}";
        public static string InstrumentPackageBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_DQS_BUCKET");
        public static string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");
        public static string OnlineFileBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_NISRA_BUCKET");
    }
}
