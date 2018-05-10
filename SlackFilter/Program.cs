using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Spin.Environment;
using Spin.Logger;

namespace SlackFilter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .ConfigureSpinEnvironmentFromAppSettings()
                .ConfigureSpinLogging(
                    new SpinLoggerConfiguration
                    {
                        GetMicrosoftLogs = false
                    })
                .UseStartup<Startup>()
                .Build();
    }
}
