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
using Blaise.Api.Storage.Services;
using Blaise.Nuget.Api.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Providers;
using Unity;
using Unity.Injection;

namespace Blaise.Api.Configuration
{
    public static class UnityConfig
    {
        public static IUnityContainer UnityContainer;

        static UnityConfig()
        {
            UnityContainer = new UnityContainer();

            //blaise api
            var blaiseConfigurationProvider = new BlaiseConfigurationProvider();
            var connectionModel = blaiseConfigurationProvider.GetConnectionModel();

            //logging
            UnityContainer.RegisterType<ILoggingService, EventLogging>();

            UnityContainer.RegisterType<IBlaiseServerParkApi, BlaiseServerParkApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseQuestionnaireApi, BlaiseQuestionnaireApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseCatiApi, BlaiseCatiApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseHealthApi, BlaiseHealthApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseRoleApi, BlaiseRoleApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseUserApi, BlaiseUserApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseFileApi, BlaiseFileApi>(new InjectionConstructor(connectionModel));
            UnityContainer.RegisterType<IBlaiseCaseApi, BlaiseCaseApi>(new InjectionConstructor(connectionModel));

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
            UnityContainer.RegisterType<IFileService, FileService>();
            UnityContainer.RegisterType<INisraFileImportService, NisraFileImportService>();
            UnityContainer.RegisterType<INisraCaseUpdateService, NisraCaseUpdateService>();
            UnityContainer.RegisterType<ICatiDataBlockService, CatiDataBlockService>();
            UnityContainer.RegisterType<INisraCaseComparisonService, NisraCaseComparisonService>();
            UnityContainer.RegisterType<IReportingService, ReportingService>();
            UnityContainer.RegisterType<IReportingServiceV2, ReportingServiceV2>();
            UnityContainer.RegisterType<ICaseService, CaseService>();
            UnityContainer.RegisterType(typeof(IRetryService<>), typeof(RetryService<>));

            //storage services
            UnityContainer.RegisterType<ICloudStorageService, CloudStorageService>();
        }

        public static T Resolve<T>()
        {
            return UnityContainer.Resolve<T>();
        }
    }
}