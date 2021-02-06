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

                    services.Configure<CourierServiceSettings>(options => host.Configuration.GetSection("Application").Bind(options));
                    services.Configure<RabbitMqTransportSettings>(options => host.Configuration.GetSection("RabbitMqTransport").Bind(options));
                    services.Configure<GrpcClientSettings>(options => host.Configuration.GetSection("Grpc").Bind(options));
                    
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        // x.AddConsumer<DispatchConsumer>();
                        // x.AddConsumer<DispatchConfirmationConsumer>();
                        // x.AddConsumer<OrderDeliveryConsumer>();
                        // x.AddConsumer<PickUpOrderConsumer>();
                        // x.AddConsumer<EnRouteToRestaurantConsumer>();
                        // x.AddConsumer<DispatchDeclinedConsumer>();
                        // x.AddConsumer<EnRouteToCustomerConsumer>();
                        // x.AddConsumer<DispatchIdentificationConsumer>();
                        // x.AddConsumer<DispatchCancellationConsumer>();

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
                            var options = context.GetService<IOptions<RabbitMqTransportSettings>>();
                            var settings = options.Value;
                            
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

    public class DispatchCancellationConsumerDefinition :
        ConsumerDefinition<DispatchCancellationConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public DispatchCancellationConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DispatchCancellationConsumer> consumerConfigurator)
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

    public class DispatchIdentificationConsumerDefinition :
        ConsumerDefinition<DispatchIdentificationConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public DispatchIdentificationConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DispatchIdentificationConsumer> consumerConfigurator)
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

    public class EnRouteToCustomerConsumerDefinition :
        ConsumerDefinition<EnRouteToCustomerConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public EnRouteToCustomerConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EnRouteToCustomerConsumer> consumerConfigurator)
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

    public class DispatchDeclinedConsumerDefinition :
        ConsumerDefinition<DispatchDeclinedConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public DispatchDeclinedConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DispatchDeclinedConsumer> consumerConfigurator)
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

    public class EnRouteToRestaurantConsumerDefinition :
        ConsumerDefinition<EnRouteToRestaurantConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public EnRouteToRestaurantConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EnRouteToRestaurantConsumer> consumerConfigurator)
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

    public class PickUpOrderConsumerDefinition :
        ConsumerDefinition<PickUpOrderConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public PickUpOrderConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PickUpOrderConsumer> consumerConfigurator)
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

    public class OrderDeliveryConsumerDefinition :
        ConsumerDefinition<OrderDeliveryConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public OrderDeliveryConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderDeliveryConsumer> consumerConfigurator)
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

    public class DispatchConfirmationConsumerDefinition :
        ConsumerDefinition<DispatchConfirmationConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public DispatchConfirmationConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DispatchConfirmationConsumer> consumerConfigurator)
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

    public class DispatchConsumerDefinition :
        ConsumerDefinition<DispatchConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public DispatchConsumerDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DispatchConsumer> consumerConfigurator)
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