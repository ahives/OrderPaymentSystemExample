namespace RestaurantService.StateMachines.Sagas
{
    using System.Collections.Generic;
    using MassTransit.EntityFrameworkCoreIntegration;
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;

    public class RestaurantServiceDbContext :
        SagaDbContext
    {
        public RestaurantServiceDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new RestaurantStateMap();
                yield return new OrderStateMap();
            }
        }
    }
}