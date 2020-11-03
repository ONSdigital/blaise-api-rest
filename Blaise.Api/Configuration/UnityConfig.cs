using Blaise.Api.Core.Interfaces;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Unity;

namespace Blaise.Api.Configuration
{
    public static class UnityConfig
    {
        public static IUnityContainer GetConfiguredContainer()
        {
			var container = new UnityContainer();

            container.RegisterType<IBlaiseApi, BlaiseApi>();
            container.RegisterType<IServerParkService, ServerParkService>();

            return container;
        }
    }
}