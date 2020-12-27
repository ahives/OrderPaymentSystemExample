namespace RestaurantService.Core.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrderItemStateMap :
        SagaClassMap<OrderItemState>
    {
        protected override void Configure(EntityTypeBuilder<OrderItemState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
            entity.Property(x => x.Timestamp);
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}