namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

    public static class BlaiseConfigurationHelper
    {
        public static string ServerParkName => ConfigurationExtensions.GetEnvironmentVariable("ServerParkName");

        public static string QuestionnaireName => ConfigurationExtensions.GetEnvironmentVariable("InstrumentName");

        public static string QuestionnaireFile => $"{QuestionnaireName}.{QuestionnaireExtension}";

        public static string QuestionnairePath => ConfigurationExtensions.GetEnvironmentVariable("InstrumentPath");

        public static string QuestionnairePackage => $"{QuestionnaireName}.{QuestionnaireExtension}";

        public static string QuestionnaireExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");

        public static string QuestionnairePackagePath => $"{QuestionnairePath}//{QuestionnairePackage}";

        public static string QuestionnairePackageBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_DQS_BUCKET");

        public static string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");

        public static string OnlineFileBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_NISRA_BUCKET");

        public static string IngestBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_INGEST_BUCKET");
    }
}
