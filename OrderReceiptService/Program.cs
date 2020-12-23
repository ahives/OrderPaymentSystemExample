namespace OrderReceiptService
{
    using System;
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Restaurant.Core;
    using Restaurant.Core.StateMachines;
    using Restaurant.Core.StateMachines.Sagas;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddMassTransit(cfg =>
            {
                cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        
                        r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                        {
                            // builder
                        });
                    });
            });
        }
    }
}