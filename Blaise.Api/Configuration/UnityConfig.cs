namespace Blaise.Api.Configuration
{
    using System.IO.Abstractions;
    using System.Web.Http.Dependencies;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Api.Core.Mappers;
    using Blaise.Api.Core.Services;
    using Blaise.Api.Logging.Services;
    using Blaise.Api.Providers;
    using Blaise.Api.Storage.Interfaces;
    using Blaise.Api.Storage.Providers;
    using Blaise.Api.Storage.Services;
    using Blaise.Nuget.Api.Api;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Blaise.Nuget.Api.Providers;
    using Unity;
    using Unity.Injection;
    using Unity.WebApi;

    public static class UnityConfig
    {
        private static readonly IUnityContainer _unityContainer;

        static UnityConfig()
        {
            _unityContainer = new UnityContainer();

            // blaise api
            var blaiseConfigurationProvider = new BlaiseConfigurationProvider();
            var connectionModel = blaiseConfigurationProvider.GetConnectionModel();

            _unityContainer.RegisterType<IBlaiseServerParkApi, BlaiseServerParkApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseQuestionnaireApi, BlaiseQuestionnaireApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseCatiApi, BlaiseCatiApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseHealthApi, BlaiseHealthApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseRoleApi, BlaiseRoleApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseUserApi, BlaiseUserApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseFileApi, BlaiseFileApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseCaseApi, BlaiseCaseApi>(new InjectionConstructor(connectionModel));
            _unityContainer.RegisterType<IBlaiseSqlApi, BlaiseSqlApi>();

            // providers
            _unityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            _unityContainer.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            _unityContainer.RegisterType<IFileSystem, FileSystem>();

            // core mappers
            _unityContainer.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            _unityContainer.RegisterType<IQuestionnaireDtoMapper, QuestionnaireDtoMapper>();
            _unityContainer.RegisterType<IQuestionnaireNodeDtoMapper, QuestionnaireNodeDtoMapper>();
            _unityContainer.RegisterType<ICatiDtoMapper, CatiDtoMapper>();
            _unityContainer.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            _unityContainer.RegisterType<IUserDtoMapper, UserDtoMapper>();
            _unityContainer.RegisterType<IQuestionnaireStatusMapper, QuestionnaireStatusMapper>();
            _unityContainer.RegisterType<IDataEntrySettingsDtoMapper, DataEntrySettingsDtoMapper>();
            _unityContainer.RegisterType<ICaseDtoMapper, CaseDtoMapper>();

            // core services
            _unityContainer.RegisterType<IServerParkService, ServerParkService>();
            _unityContainer.RegisterType<IQuestionnaireService, QuestionnaireService>();
            _unityContainer.RegisterType<IQuestionnaireDataService, QuestionnaireDataService>();
            _unityContainer.RegisterType<IQuestionnaireInstallerService, QuestionnaireInstallerService>();
            _unityContainer.RegisterType<IQuestionnaireUninstallerService, QuestionnaireUninstallerService>();
            _unityContainer.RegisterType<ICatiService, CatiService>();
            _unityContainer.RegisterType<IHealthCheckService, HealthCheckService>();
            _unityContainer.RegisterType<IUserRoleService, UserRoleService>();
            _unityContainer.RegisterType<IUserService, UserService>();
            _unityContainer.RegisterType<IFileService, FileService>();
            _unityContainer.RegisterType<INisraFileImportService, NisraFileImportService>();
            _unityContainer.RegisterType<INisraCaseUpdateService, NisraCaseUpdateService>();
            _unityContainer.RegisterType<ICatiDataBlockService, CatiDataBlockService>();
            _unityContainer.RegisterType<INisraCaseComparisonService, NisraCaseComparisonService>();
            _unityContainer.RegisterType<IReportingService, ReportingService>();
            _unityContainer.RegisterType<ICaseService, CaseService>();
            _unityContainer.RegisterType<IIngestService, IngestService>();
            _unityContainer.RegisterType(typeof(IRetryService<>), typeof(RetryService<>));

            // storage service
            _unityContainer.RegisterType<ICloudStorageService, CloudStorageService>();

            // logging service
            _unityContainer.RegisterType<ILoggingService, EventLogging>();
        }

        public static T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        // testing
        public static IUnityContainer GetContainer()
        {
            return _unityContainer;
        }

        // production
        public static IDependencyResolver GetDependencyResolver()
        {
            return new UnityDependencyResolver(_unityContainer);
        }
    }
}
