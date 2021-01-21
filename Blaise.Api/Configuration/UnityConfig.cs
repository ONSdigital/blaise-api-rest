using System.IO.Abstractions;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Mappers;
using Blaise.Api.Core.Services;
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

            container.RegisterType<IBlaiseServerParkApi, BlaiseServerParkApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseSurveyApi, BlaiseSurveyApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseCatiApi, BlaiseCatiApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseHealthApi, BlaiseHealthApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseRoleApi, BlaiseRoleApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseUserApi, BlaiseUserApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseFileApi, BlaiseFileApi>(new InjectionConstructor(connectionModel));
            container.RegisterType<IBlaiseCaseApi, BlaiseCaseApi>(new InjectionConstructor(connectionModel));

            //providers
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            container.RegisterType<IFileSystem, FileSystem>();

            //core mappers
            container.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            container.RegisterType<IInstrumentDtoMapper, InstrumentDtoMapper>();
            container.RegisterType<ICatiInstrumentDtoMapper, CatiInstrumentDtoMapper>();
            container.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            container.RegisterType<IUserDtoMapper, UserDtoMapper>();

            //core services
            container.RegisterType<IServerParkService, ServerParkService>();
            container.RegisterType<IInstrumentService, InstrumentService>();
            container.RegisterType<IInstallInstrumentService, InstallInstrumentService>();
            container.RegisterType<IUninstallInstrumentService, UninstallInstrumentService>();
            container.RegisterType<ICatiService, CatiService>();
            container.RegisterType<IHealthCheckService, HealthCheckService>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IBlaiseFileService, BlaiseFileService>();

            //storage services
            container.RegisterType<ICloudStorageService, CloudStorageService>();

            return container;
        }
    }
}