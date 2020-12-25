﻿namespace CourierService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Consumers;
    using Data.Core;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using StateMachines;
    using StateMachines.Sagas;

    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .UseSerilog((host, log) =>
                {
                    string? appBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    log.MinimumLevel.Information();
                    log.WriteTo.File($"{appBin}/log/log-{DateTime.Now:yyMMdd_HHmmss}.txt");
                    log.WriteTo.Console(LogEventLevel.Debug);
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddDbContext<OrdersDbContext>(x =>
                        x.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection")));
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<CourierDispatchConsumer>();
                        
                        x.SetKebabCaseEndpointNameFormatter();
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            string vhost = host.Configuration
                                .GetSection("Application")
                                .GetValue<string>("VirtualHost");
                            
                            cfg.Host("localhost", vhost, h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                            
                            cfg.ConfigureEndpoints(context);
                            // cfg.UseMessageRetry(x => x.SetRetryPolicy(new RetryPolicyFactory()));
                        });

                        x.AddSagaStateMachine<CourierStateMachine, CourierState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                
                                r.AddDbContext<DbContext, CourierStateDbContext>((provider, builder) =>
                                {
                                    builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(CourierStateDbContext)}");
                                    });
                                });
                            });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}