using System.ServiceProcess;

namespace Blaise.Api
{
    internal class Program
    {
        private static void Main()
        {
#if DEBUG
            var apiService = new ApiService();
            apiService.OnDebug();
#else
            var servicesToRun = new ServiceBase[]
            {
                new ApiService()
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}