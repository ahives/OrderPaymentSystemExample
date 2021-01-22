namespace OrderProcessingService.Core.StateMachines.Sagas
{
    using System.Collections.Generic;
    using MassTransit.EntityFrameworkCoreIntegration;
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;

    public class OrderProcessingServiceDbContext :
        SagaDbContext
    {
        public OrderProcessingServiceDbContext(DbContextOptions<OrderProcessingServiceDbContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new OrderStateMap();
                yield return new OrderItemStateMap();
            }
        }
    }
}