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
            UnityContainer.RegisterSingleton<IBlaiseSurveyApi, BlaiseSurveyApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseCatiApi, BlaiseCatiApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseHealthApi, BlaiseHealthApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseRoleApi, BlaiseRoleApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseUserApi, BlaiseUserApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseFileApi, BlaiseFileApiStub>();
            UnityContainer.RegisterSingleton<IBlaiseCaseApi, BlaiseCaseApiStub>();

            //providers
            UnityContainer.RegisterType<IConfigurationProvider, ConfigurationProvider>();
            UnityContainer.RegisterType<ICloudStorageClientProvider, CloudStorageClientProvider>();
            UnityContainer.RegisterType<IFileSystem, FileSystem>();

            //core mappers
            UnityContainer.RegisterType<IServerParkDtoMapper, ServerParkDtoMapper>();
            UnityContainer.RegisterType<IInstrumentDtoMapper, InstrumentDtoMapper>();
            UnityContainer.RegisterType<IInstrumentNodeDtoMapper, InstrumentNodeDtoMapper>();
            UnityContainer.RegisterType<ICatiDtoMapper, CatiDtoMapper>();
            UnityContainer.RegisterType<IUserRoleDtoMapper, UserRoleDtoMapper>();
            UnityContainer.RegisterType<IUserDtoMapper, UserDtoMapper>();
            UnityContainer.RegisterType<IInstrumentStatusMapper, InstrumentStatusMapper>();
            UnityContainer.RegisterType<IDataEntrySettingsDtoMapper, DataEntrySettingsDtoMapper>();

            //core services
            UnityContainer.RegisterType<IServerParkService, ServerParkService>();
            UnityContainer.RegisterType<IInstrumentService, InstrumentService>();
            UnityContainer.RegisterType<IInstrumentDataService, InstrumentDataService>();
            UnityContainer.RegisterType<IInstrumentInstallerService, InstrumentInstallerService>();
            UnityContainer.RegisterType<IInstrumentUninstallerService, InstrumentUninstallerService>();
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