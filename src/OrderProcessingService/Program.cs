namespace OrderProcessingService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core;
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
                    log.WriteTo.Console(LogEventLevel.Information);
                })
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    // services.AddScoped<IShelfManager, ShelfManager>();
                    services.AddSingleton<IGrpcClient<IOrderProcessor>, OrderProcessorClient>();

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
                        var config = new OrderProcessingServiceSettings();

                        host.Configuration.Bind("Application", config);

                        return config;
                    });

                    services.AddDbContext<OrderProcessingServiceDbContext>(builder =>
                        builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                        {
                            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(OrderProcessingServiceDbContext)}");
                        }));
                    
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();
                        
                        x.AddConsumer(typeof(PrepareOrderItemConsumer), typeof(PrepareOrderItemConsumerDefinition));
                        x.AddConsumer(typeof(PrepareOrderConsumer), typeof(PrepareOrderConsumerDefinition));
                        x.AddConsumer(typeof(CancelOrderItemConsumer), typeof(CancelOrderItemConsumerDefinition));
                        x.AddConsumer(typeof(CancelOrderConsumer), typeof(CancelOrderConsumerDefinition));
                        x.AddConsumer(typeof(VoidOrderItemConsumer), typeof(VoidOrderItemConsumerDefinition));
                        x.AddConsumer(typeof(OrderNotFoundConsumer), typeof(OrderNotFoundConsumerDefinition));

                        x.AddSagaStateMachine(typeof(OrderStateMachine), typeof(OrderStateDefinition));
                        x.AddSagaStateMachine(typeof(OrderItemStateMachine), typeof(OrderItemStateDefinition));
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var settings = context.GetService<RabbitMqTransportSettings>();
                            
                            cfg.Host(settings.Host, settings.VirtualHost, h =>
                            {
                                h.Username(settings.Username);
                                h.Password(settings.Password);
                            });
                            
                            cfg.ConfigureEndpoints(context);
                            // cfg.UseMessageRetry(x => x.SetRetryPolicy(new RetryPolicyFactory()));
                        });

                        x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.LockStatementProvider = new PostgresLockStatementProvider();
                                r.ExistingDbContext<OrderProcessingServiceDbContext>();
                            });
                        
                        x.AddSagaStateMachine<OrderItemStateMachine, OrderItemState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.LockStatementProvider = new PostgresLockStatementProvider();
                                r.ExistingDbContext<OrderProcessingServiceDbContext>();
                            });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}