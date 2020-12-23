namespace OrderReceiptService
{
    using MassTransit;
    using MassTransit.EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using StateMachines;
    using StateMachines.Sagas;

    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddMassTransit(cfg =>
            {
                cfg.AddSagaStateMachine<RestaurantStateMachine, RestaurantState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        
                        r.AddDbContext<DbContext, RestaurantStateDbContext>((provider, builder) =>
                        {
                            // builder
                        });
                    });
            });
        }
    }
}