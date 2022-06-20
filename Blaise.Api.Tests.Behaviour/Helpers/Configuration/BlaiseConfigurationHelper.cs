using Blaise.Api.Tests.Behaviour.Helpers.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Configuration
{
    public static class BlaiseConfigurationHelper
    {
        public static bool UseStubs => StubConfigurationHelper.UseStubs;
        public static string ServerParkName => UseStubs ? "gustyTest" : ConfigurationExtensions.GetEnvironmentVariable("ServerParkName");
        public static string QuestionnaireName => UseStubs ? "opnTest" : ConfigurationExtensions.GetEnvironmentVariable("InstrumentName");
        public static string QuestionnaireFile => UseStubs ? "instrumentFileTest.bkpg" : $"{QuestionnaireName}.{QuestionnaireExtension}";
        public static string QuestionnairePath => ConfigurationExtensions.GetEnvironmentVariable("InstrumentPath");
        public static string QuestionnairePackage => $"{QuestionnaireName}.{QuestionnaireExtension}";
        public static string QuestionnaireExtension => ConfigurationExtensions.GetVariable("PACKAGE_EXTENSION");
        public static string QuestionnairePackagePath => $"{QuestionnairePath}//{QuestionnairePackage}";
        public static string QuestionnairePackageBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_DQS_BUCKET");
        public static string TempPath => ConfigurationExtensions.GetVariable("TEMP_PATH");
        public static string OnlineFileBucket => ConfigurationExtensions.GetVariable("ENV_BLAISE_NISRA_BUCKET");
    }
}
