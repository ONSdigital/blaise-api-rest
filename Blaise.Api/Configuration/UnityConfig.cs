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
        public static IUnityContainer GetConfiguredContainer()
        {
            var container = new UnityContainer();

            //blaise api
            var blaiseConfigurationProvider = new BlaiseConfigurationProvider();
            var connectionModel = blaiseConfigurationProvider.GetConnectionModel();

            //logging
            container.RegisterType<ILoggingService, EventLogging>();

            container.RegisterSingleton<IBlaiseServerParkApi, BlaiseServerParkApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseSurveyApi, BlaiseSurveyApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseCatiApi, BlaiseCatiApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseHealthApi, BlaiseHealthApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseRoleApi, BlaiseRoleApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseUserApi, BlaiseUserApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseFileApi, BlaiseFileApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseCaseApi, BlaiseCaseApi>(new InjectionConstructor(connectionModel));
            container.RegisterSingleton<IBlaiseAdminApi, BlaiseAdminApi>();

            //providers
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            container.RegisterType<IFileSystem, FileSystem>();

            //core mappers
            container.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            container.RegisterType<IInstrumentDtoMapper, InstrumentDtoMapper>();
            container.RegisterType<IInstrumentNodeDtoMapper, InstrumentNodeDtoMapper>();
            container.RegisterType<ICatiInstrumentDtoMapper, CatiInstrumentDtoMapper>();
            container.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            container.RegisterType<IUserDtoMapper, UserDtoMapper>();
            container.RegisterType<IInstrumentStatusMapper, InstrumentStatusMapper>();
            container.RegisterType<IOpenConnectionModelMapper, OpenConnectionModelMapper>();

            //core services
            container.RegisterType<IServerParkService, ServerParkService>();
            container.RegisterType<IInstrumentService, InstrumentService>();
            container.RegisterType<IInstrumentDataService, InstrumentDataService>();
            container.RegisterType<IInstrumentInstallerService, InstrumentInstallerService>();
            container.RegisterType<IInstrumentUninstallerService, InstrumentUninstallerService>();
            container.RegisterType<ICatiService, CatiService>();
            container.RegisterType<IHealthCheckService, HealthCheckService>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<INisraFileImportService, NisraFileImportService>();
            container.RegisterType<INisraCaseUpdateService, NisraCaseUpdateService>();
            container.RegisterType<ICatiDataBlockService, CatiDataBlockService>();
            container.RegisterType<INisraCaseComparisonService, NisraCaseComparisonService>();
            container.RegisterType<IReportingService, ReportingService>();
            container.RegisterType<IAdminService, AdminService>();

            //storage services
            container.RegisterType<ICloudStorageService, CloudStorageService>();

            return container;
        }

        public static T Resolve<T>()
        {
            var container = GetConfiguredContainer();

            return container.Resolve<T>();
        }
    }
}