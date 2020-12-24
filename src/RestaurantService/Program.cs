namespace RestaurantService
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Consumers;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
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
                    log.WriteTo.Console(LogEventLevel.Information);
                })
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<OrderValidationConsumer>();
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ReceiveEndpoint("order-validator", e =>
                            {
                                e.ConfigureConsumer<OrderValidationConsumer>(context);
                            });
                        });

                        x.AddSagaStateMachine<RestaurantStateMachine, RestaurantState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                                
                                r.AddDbContext<DbContext, RestaurantStateDbContext>((provider, builder) =>
                                {
                                    builder.UseNpgsql(host.Configuration.GetConnectionString("OrdersConnection"), m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(RestaurantStateDbContext)}");
                                    });
                                });
                            });
                    });

                    services.AddMassTransitHostedService();
                });

    }
}