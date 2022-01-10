using Unity;

namespace Blaise.Api.Tests.Behaviour.Stubs
{
    class StartupStub : Startup
    {
        public override IUnityContainer UnityContainer => UnityConfigStub.UnityContainer;
    }
}
