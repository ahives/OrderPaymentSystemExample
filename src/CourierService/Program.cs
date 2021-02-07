namespace CourierService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Configuration;
    using Core.Consumers;
    using Core.StateMachines;
    using Core.StateMachines.Sagas;
    using Definitions;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Configuration;
    using Services.Core.Configuration;

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
                    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
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
                    services.AddSingleton<IGrpcClient<ICourierDispatcher>, CourierDispatcherClient>();

                    services.AddSingleton(x =>
                    {
                        var config = new GrpcClientSettings();

                        host.Configuration.Bind("Grpc", config);

                        return config;
                    });

                    services.AddSingleton(x =>
                    {
                        var config = new RabbitMqTransportSettings();

                        host.Configuration.Bind("RabbitMqTransport", config);

                        return config;
                    });

                    services.AddSingleton(x =>
                    {
                        var config = new CourierServiceSettings();

                        host.Configuration.Bind("Application", config);

                        return config;
                    });
                    
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        x.AddConsumer(typeof(DispatchConsumer), typeof(DispatchConsumerDefinition));
                        x.AddConsumer(typeof(DispatchConfirmationConsumer), typeof(DispatchConfirmationConsumerDefinition));
                        x.AddConsumer(typeof(OrderDeliveryConsumer), typeof(OrderDeliveryConsumerDefinition));
                        x.AddConsumer(typeof(PickUpOrderConsumer), typeof(PickUpOrderConsumerDefinition));
                        x.AddConsumer(typeof(EnRouteToRestaurantConsumer), typeof(EnRouteToRestaurantConsumerDefinition));
                        x.AddConsumer(typeof(DispatchDeclinedConsumer), typeof(DispatchDeclinedConsumerDefinition));
                        x.AddConsumer(typeof(EnRouteToCustomerConsumer), typeof(EnRouteToCustomerConsumerDefinition));
                        x.AddConsumer(typeof(DispatchIdentificationConsumer), typeof(DispatchIdentificationConsumerDefinition));
                        x.AddConsumer(typeof(DispatchCancellationConsumer), typeof(DispatchCancellationConsumerDefinition));
                        
                        x.AddSagaStateMachine(typeof(CourierStateMachine), typeof(CourierStateDefinition));
                        
                        Uri schedulerEndpoint = new Uri("queue:quartz");
                        
                        x.AddMessageScheduler(schedulerEndpoint);
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var settings = context.GetService<RabbitMqTransportSettings>();
                            
                            cfg.Host(settings.Host, settings.VirtualHost, h =>
                            {
                                h.Username(settings.Username);
                                h.Password(settings.Password);
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