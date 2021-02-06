namespace CourierWebService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Serilog;
    using Serilog.Events;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    string appBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    log.MinimumLevel.Information();
                    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                    log.WriteTo.File($"{appBin}/log/log-{DateTime.Now:yyMMdd_HHmmss}.txt");
                    log.WriteTo.Console(LogEventLevel.Information);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxConcurrentConnections = 100;
                        options.Limits.MaxRequestBodySize = 10 * 1024;
                        options.Limits.MinRequestBodyDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
                        options.Limits.MinResponseDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
                        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                        options.ListenLocalhost(5010);
                    });
                    
                    webBuilder.UseStartup<Startup>();
                });
    }
}