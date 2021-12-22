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
        public static IUnityContainer GetConfiguredContainer()
        {
            var container = new UnityContainer();

            //logging
            container.RegisterType<ILoggingService, TestLogging>();

            container.RegisterType<IBlaiseServerParkApi, BlaiseServerParkApiStub>();
            container.RegisterType<IBlaiseSurveyApi, BlaiseSurveyApiStub>();
            container.RegisterType<IBlaiseCatiApi, BlaiseCatiApiStub>();
            container.RegisterType<IBlaiseHealthApi, BlaiseHealthApiStub>();
            container.RegisterType<IBlaiseRoleApi, BlaiseRoleApiStub>();
            container.RegisterType<IBlaiseUserApi, BlaiseUserApiStub>();
            container.RegisterType<IBlaiseFileApi, BlaiseFileApiStub>();
            container.RegisterType<IBlaiseCaseApi, BlaiseCaseApiStub>();

            //providers
            container.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            container.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            container.RegisterType<IFileSystem, FileSystem>();

            //core mappers
            container.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            container.RegisterType<IInstrumentDtoMapper, InstrumentDtoMapper>();
            container.RegisterType<IInstrumentNodeDtoMapper, InstrumentNodeDtoMapper>();
            container.RegisterType<ICatiDtoMapper, CatiDtoMapper>();
            container.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            container.RegisterType<IUserDtoMapper, UserDtoMapper>();
            container.RegisterType<IInstrumentStatusMapper, InstrumentStatusMapper>();
            container.RegisterType<IDataEntrySettingsDtoMapper, DataEntrySettingsDtoMapper>();

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
            container.RegisterType<IFileService, FileServiceStub>();
            container.RegisterType<INisraFileImportService, NisraFileImportService>();
            container.RegisterType<INisraCaseUpdateService, NisraCaseUpdateService>();
            container.RegisterType<ICatiDataBlockService, CatiDataBlockService>();
            container.RegisterType<INisraCaseComparisonService, NisraCaseComparisonService>();
            container.RegisterType<IReportingService, ReportingService>();
            container.RegisterType<ICaseService, CaseService>();

            //storage services
            container.RegisterType<ICloudStorageService, CloudStorageServiceStub>();
            return container;
        }

        public static T Resolve<T>()
        {
            var container = GetConfiguredContainer();

            return container.Resolve<T>();
        }
    }
}