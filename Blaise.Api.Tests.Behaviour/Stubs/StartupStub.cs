using Unity;

namespace Blaise.Api.Tests.Behaviour.Stubs
{
    internal class StartupStub : Startup
    {
        public override IUnityContainer UnityContainer => UnityConfigStub.UnityContainer;
    }
}
