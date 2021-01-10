namespace CourierService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Consumers;
    using Core.StateMachines;
    using Core.StateMachines.Sagas;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using Service.Grpc.Core;

    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .UseSerilog((host, log) =>
                {
                    string appBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    log.MinimumLevel.Information();
                    log.WriteTo.File($"{appBin}/log/log-{DateTime.Now:yyMMdd_HHmmss}.txt");
                    log.WriteTo.Console(LogEventLevel.Debug);
                })
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddSingleton<ICourierDispatcherClient, CourierDispatcherClient>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<CourierDispatchConsumer>();
                        x.AddConsumer<CourierDispatchConfirmationConsumer>();
                        x.AddConsumer<OrderDeliveryConsumer>();
                        x.AddConsumer<PickUpOrderConsumer>();
                        x.AddConsumer<CourierEnRouteToRestaurantConsumer>();
                        x.AddConsumer<CourierDispatchDeclinedConsumer>();
                        x.AddConsumer<UpdateCourierStatusConsumer>();
                        
                        x.SetKebabCaseEndpointNameFormatter();

                        Uri schedulerEndpoint = new Uri("queue:scheduler");
                        
                        x.AddMessageScheduler(schedulerEndpoint);
                        
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
                            
                            cfg.UseMessageScheduler(schedulerEndpoint);
                            
                            cfg.ConfigureEndpoints(context);
                            // cfg.UseMessageRetry(x => x.SetRetryPolicy(new RetryPolicyFactory()));
                        });

                        x.AddSagaStateMachine<CourierStateMachine, CourierState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                
                                r.AddDbContext<DbContext, CourierServiceDbContext>((provider, builder) =>
                                {
                                    builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(CourierServiceDbContext)}");
                                    });
                                });
                            });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}