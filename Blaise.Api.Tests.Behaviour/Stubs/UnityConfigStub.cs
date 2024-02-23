using System.IO.Abstractions;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Mappers;
using Blaise.Api.Core.Services;
using Blaise.Api.Logging.Services;
using Blaise.Api.Providers;
using Blaise.Api.Storage.Interfaces;
using Blaise.Api.Storage.Providers;
using Blaise.Api.Tests.Behaviour.Stubs.Blaise;
using Blaise.Api.Tests.Behaviour.Stubs.IO;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Unity;

namespace Blaise.Api.Tests.Behaviour.Stubs
{
    public static class UnityConfigStub
    {
        public static IUnityContainer UnityContainer;

        static UnityConfigStub()
        {
            UnityContainer = new UnityContainer();

            //logging
            UnityContainer.RegisterSingleton<ILoggingService, ConsoleLogging>();

            UnityContainer.RegisterSingleton<IBlaiseServerParkApi, BlaiseServerParkApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseQuestionnaireApi, BlaiseQuestionnaireApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseCatiApi, BlaiseCatiApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseHealthApi, BlaiseHealthApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseRoleApi, BlaiseRoleApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseUserApi, BlaiseUserApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseFileApi, BlaiseFileApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseCaseApi, BlaiseCaseApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseSqlApi, BlaiseSqlApiStub>();

            //providers
            UnityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            UnityContainer.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            UnityContainer.RegisterType<IFileSystem, FileSystem>();

            //core mappers
            UnityContainer.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            UnityContainer.RegisterType<IQuestionnaireDtoMapper, QuestionnaireDtoMapper>();
            UnityContainer.RegisterType<IQuestionnaireNodeDtoMapper, QuestionnaireNodeDtoMapper>();
            UnityContainer.RegisterType<ICatiDtoMapper, CatiDtoMapper>();
            UnityContainer.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            UnityContainer.RegisterType<IUserDtoMapper, UserDtoMapper>();
            UnityContainer.RegisterType<IQuestionnaireStatusMapper, QuestionnaireStatusMapper>();
            UnityContainer.RegisterType<IDataEntrySettingsDtoMapper, DataEntrySettingsDtoMapper>();

            //core services
            UnityContainer.RegisterType<IServerParkService, ServerParkService>();
            UnityContainer.RegisterType<IQuestionnaireService, QuestionnaireService>();
            UnityContainer.RegisterType<IQuestionnaireDataService, QuestionnaireDataService>();
            UnityContainer.RegisterType<IQuestionnaireInstallerService, QuestionnaireInstallerService>();
            UnityContainer.RegisterType<IQuestionnaireUninstallerService, QuestionnaireUninstallerService>();
            UnityContainer.RegisterType<ICatiService, CatiService>();
            UnityContainer.RegisterType<IHealthCheckService, HealthCheckService>();
            UnityContainer.RegisterType<IUserRoleService, UserRoleService>();
            UnityContainer.RegisterType<IUserService, UserService>();
            UnityContainer.RegisterSingleton<IFileService, FileServiceStub>();
            UnityContainer.RegisterType<INisraFileImportService, NisraFileImportService>();
            UnityContainer.RegisterType<INisraCaseUpdateService, NisraCaseUpdateService>();
            UnityContainer.RegisterType<ICatiDataBlockService, CatiDataBlockService>();
            UnityContainer.RegisterType<INisraCaseComparisonService, NisraCaseComparisonService>();
            UnityContainer.RegisterType<IReportingService, ReportingService>();
            UnityContainer.RegisterType<ICaseService, CaseService>();
            UnityContainer.RegisterType(typeof(IRetryService<>), typeof(RetryService<>));

            //storage services
            UnityContainer.RegisterSingleton<ICloudStorageService, CloudStorageServiceStub>();
        }

        public static T Resolve<T>()
        {
            return UnityContainer.Resolve<T>();
        }
    }
}