namespace CourierService
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
                    services.AddSingleton<IGrpcClient<ICourierDispatcher>, CourierDispatcherClient>();

                    services.Configure<CourierServiceSettings>(options => host.Configuration.GetSection("Application").Bind(options));
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<DispatchConsumer>();
                        x.AddConsumer<DispatchConfirmationConsumer>();
                        x.AddConsumer<OrderDeliveryConsumer>();
                        x.AddConsumer<PickUpOrderConsumer>();
                        x.AddConsumer<EnRouteToRestaurantConsumer>();
                        x.AddConsumer<DispatchDeclinedConsumer>();
                        x.AddConsumer<EnRouteToCustomerConsumer>();
                        x.AddConsumer<DispatchIdentificationConsumer>();
                        x.AddConsumer<DispatchCancellationConsumer>();
                        
                        x.SetKebabCaseEndpointNameFormatter();

                        Uri schedulerEndpoint = new Uri("queue:quartz");
                        
                        x.AddMessageScheduler(schedulerEndpoint);
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var options = context.GetService<IOptions<CourierServiceSettings>>();
                            var settings = options.Value;
                            
                            cfg.Host("localhost", settings.VirtualHost, h =>
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