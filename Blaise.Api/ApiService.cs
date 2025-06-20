using System;
using System.ServiceProcess;
using Blaise.Api.Providers;
using Microsoft.Owin.Hosting;
namespace Blaise.Api
{
    internal partial class ApiService : ServiceBase
    {
        private IDisposable _server;

        public ApiService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
            Console.ReadLine();
        }

        protected override void OnStart(string[] args)
        {
            var configurationProvider = new ConfigurationProvider();
            _server = WebApp.Start<Startup>(configurationProvider.BaseUrl);
        }

        protected override void OnStop()
        {
            _server?.Dispose();
            base.OnStop();
        }
    }
}
