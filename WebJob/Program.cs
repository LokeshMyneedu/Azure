using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            //C# .NET Core. WebJobs SDK v3.0
            //you need to add Microsoft.Extensions.Hosting and Microsoft.Extensions.Logging
            //namespaces to your code
            var builder = new HostBuilder();
            builder.UseEnvironment("development");
            builder.ConfigureWebJobs(wj =>
            {
                wj.AddAzureStorageCoreServices();
                wj.AddAzureStorage();
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });
            var host = builder.Build();
            using (host)
            {
                host.Run();
            }

               
        }
    }
}
