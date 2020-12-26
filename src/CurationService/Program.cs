namespace CurationService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Consumers;
    using Core.Jobs;
    using Data.Core;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
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
            Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    string? appBin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    
                    log.MinimumLevel.Information();
                    log.WriteTo.File($"{appBin}/log/log-{DateTime.Now:yyMMdd_HHmmss}.txt");
                    log.WriteTo.Console(LogEventLevel.Information);
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddSingleton<IExpireOrders, ExpireOrders>();
                    services.AddSingleton<IOrderExpiryCalculator, OrderExpiryCalculator>();
                    
                    services.AddDbContext<OrdersDbContext>(x =>
                        x.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection")));
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<OrderCurationConsumer>();
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ReceiveEndpoint("order-curation", e =>
                            {
                                e.ConfigureConsumer<OrderCurationConsumer>(context);
                            });
                        });
                    });

                    services.AddMassTransitHostedService();
                    
                    services.AddQuartz(q =>
                    {
                        q.UseJobFactory<KitchenJobFactory>(x =>
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
                        
                        q.ScheduleJob<OrderCurationJob>(t =>
                            t.WithIdentity("shelf-curator")
                                .StartNow()
                                .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(10))
                                .WithDescription(""));
                        
                        q.ScheduleJob<OrderExpiryJob>(t =>
                            t.WithIdentity("order-expiry")
                                .StartNow()
                                .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(5))
                                .WithDescription(""));
                    });

                    services.AddQuartzHostedService(q =>
                    {
                        q.WaitForJobsToComplete = true;
                    });
                });
    }
}