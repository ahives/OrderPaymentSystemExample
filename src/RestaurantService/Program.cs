namespace RestaurantService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core;
    using Core.Consumers;
    using Core.StateMachines;
    using Core.StateMachines.Sagas;
    using Data.Core;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Serilog;
    using Serilog.Events;
    using Services.Core;

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
                    services.AddSingleton<IOrderValidator, OrderValidator>();
                    services.AddSingleton<IKitchenManager, KitchenManager>();
                    services.AddSingleton<IPrepareOrder, PrepareOrder>();
                    services.AddSingleton<ILowInventoryDetector, LowInventoryDetector>();
                    
                    services.AddDbContext<OrdersDbContext>(x =>
                        x.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection")),
                        ServiceLifetime.Singleton);

                    services.AddDbContext<RestaurantServiceDbContext>(builder =>
                        builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                        {
                            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(RestaurantServiceDbContext)}");
                        }), ServiceLifetime.Singleton);
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<OrderValidationConsumer>();
                        x.AddConsumer<PrepareOrderItemConsumer>();
                        x.AddConsumer<ReceiptConfirmationConsumer>();
                        x.AddConsumer<CancelOrderRequestConsumer>();
                        
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

                        x.AddSagaStateMachine<RestaurantStateMachine, RestaurantState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.LockStatementProvider = new PostgresLockStatementProvider();
                                r.ExistingDbContext<RestaurantServiceDbContext>();
                            });

                        x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.LockStatementProvider = new PostgresLockStatementProvider();
                                r.ExistingDbContext<RestaurantServiceDbContext>();
                            });

                        x.AddSagaStateMachine<OrderItemStateMachine, OrderItemState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.LockStatementProvider = new PostgresLockStatementProvider();
                                r.ExistingDbContext<RestaurantServiceDbContext>();
                            });
                    });

                    services.AddMassTransitHostedService();
                    
                    services.AddQuartz(q =>
                    {
                        q.UseJobFactory<RestaurantJobFactory>(x =>
                        {
                            x.CreateScope = true;
                            x.AllowDefaultConstructor = true;
                        });
                        
                        q.UsePersistentStore(s =>
                        {
                            s.UseProperties = true;
                            s.RetryInterval = TimeSpan.FromSeconds(15);
                            s.UsePostgres(db =>
                            {
                                db.ConnectionString = host.Configuration.GetConnectionString("QuartzConnection");
                                db.TablePrefix = "qrtz_";
                            });
                            s.UseJsonSerializer();
                        });
                        
                        q.ScheduleJob<LowInventoryDetectorJob>(t =>
                            t.WithIdentity("low-inventory")
                                .StartNow()
                                .WithDailyTimeIntervalSchedule(x => x.WithIntervalInMinutes(1))
                                .WithDescription(""));
                    });

                    services.AddQuartzHostedService(q =>
                    {
                        q.WaitForJobsToComplete = true;
                    });
                });
    }
}