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
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;
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
                        
                        // x.AddConsumer<PrepareOrderItemConsumer>();
                        // x.AddConsumer<PrepareOrderConsumer>();
                        // x.AddConsumer<CancelOrderItemConsumer>();
                        // x.AddConsumer<CancelOrderConsumer>();
                        // x.AddConsumer<VoidOrderItemConsumer>();
                        
                        x.AddConsumer(typeof(PrepareOrderItemConsumer), typeof(PrepareOrderItemConsumerDefinition));
                        x.AddConsumer(typeof(PrepareOrderConsumer), typeof(PrepareOrderConsumerDefinition));
                        x.AddConsumer(typeof(CancelOrderItemConsumer), typeof(CancelOrderItemConsumerDefinition));
                        x.AddConsumer(typeof(CancelOrderConsumer), typeof(CancelOrderConsumerDefinition));
                        x.AddConsumer(typeof(VoidOrderItemConsumer), typeof(VoidOrderItemConsumerDefinition));

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

    public class PrepareOrderItemConsumerDefinition :
        ConsumerDefinition<PrepareOrderItemConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public PrepareOrderItemConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PrepareOrderItemConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }

    public class PrepareOrderConsumerDefinition :
        ConsumerDefinition<PrepareOrderConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public PrepareOrderConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PrepareOrderConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }

    public class CancelOrderItemConsumerDefinition :
        ConsumerDefinition<CancelOrderItemConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public CancelOrderItemConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CancelOrderItemConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }

    public class CancelOrderConsumerDefinition :
        ConsumerDefinition<CancelOrderConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public CancelOrderConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CancelOrderConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }

    public class VoidOrderItemConsumerDefinition :
        ConsumerDefinition<VoidOrderItemConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public VoidOrderItemConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<VoidOrderItemConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}