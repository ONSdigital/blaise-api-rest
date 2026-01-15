namespace Blaise.Api
{
    using System.ServiceProcess;

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
                new ApiService(),
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
