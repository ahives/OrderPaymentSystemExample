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
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
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

                    services.Configure<OrderProcessingServiceSettings>(options => host.Configuration.GetSection("Application").Bind(options));
                    services.Configure<RabbitMqTransportSettings>(options => host.Configuration.GetSection("RabbitMqTransport").Bind(options));
                    services.Configure<GrpcClientSettings>(options => host.Configuration.GetSection("Grpc").Bind(options));

                    services.AddDbContext<OrderProcessingServiceDbContext>(builder =>
                        builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                        {
                            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(OrderProcessingServiceDbContext)}");
                        }));
                    
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();
                        
                        x.AddConsumer<PrepareOrderItemConsumer>();
                        x.AddConsumer<PrepareOrderConsumer>();
                        x.AddConsumer<CancelOrderItemConsumer>();
                        x.AddConsumer<CancelOrderConsumer>();
                        x.AddConsumer<VoidOrderItemConsumer>();
                        
                        x.AddSagaStateMachine(typeof(OrderStateMachine), typeof(OrderStateDefinition));
                        x.AddSagaStateMachine(typeof(OrderItemStateMachine), typeof(OrderItemStateDefinition));
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var options = context.GetService<IOptions<RabbitMqTransportSettings>>();
                            var settings = options.Value;
                            
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