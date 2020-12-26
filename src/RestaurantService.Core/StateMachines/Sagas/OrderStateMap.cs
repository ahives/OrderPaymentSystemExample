namespace RestaurantService.Core.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrderStateMap :
        SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
            entity.Property(x => x.CustomerId);
            entity.Property(x => x.CourierId);
            entity.Property(x => x.Timestamp);
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}