using System;
using System.Threading;
using Blaise.Api.Providers;
using Microsoft.Owin.Hosting;

namespace Blaise.Api
{
    internal class Program
    {
        private static void Main()
        {
            var baseUrl = ConfigurationProvider.BaseUrl;

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseUrl))
            {
                Console.WriteLine($"Starting Blaise RESTful API service on '{baseUrl}'");
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
