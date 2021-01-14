namespace MassTransitSchedulerService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz.Impl;
    using Serilog;
    using Serilog.Events;

    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    string? appBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    
                    log.MinimumLevel.Information();
                    log.WriteTo.File($"{appBin}/log/log-{DateTime.Now:yyMMdd_HHmmss}.txt");
                    log.WriteTo.Console(LogEventLevel.Information);
                })
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    services.Configure<OtherOptions>(host.Configuration);
                    services.Configure<QuartzOptions>(host.Configuration.GetSection("Quartz"));
                    
                    services.AddSingleton<QuartzConfiguration>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var options = context.GetService<QuartzConfiguration>();
                            
                            cfg.AddScheduling(s =>
                            {
                                s.SchedulerFactory = new StdSchedulerFactory(options.Configuration);
                                s.QueueName = options.Queue;
                            });
                            
                            string vhost = host.Configuration
                                .GetSection("Application")
                                .GetValue<string>("VirtualHost");
                            
                            cfg.Host("localhost", vhost, h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}