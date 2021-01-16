namespace OrderProcessingService.Core.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrderStateMap :
        SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).IsRequired();
            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.CourierId);
            entity.Property(x => x.RestaurantId).IsRequired();
            entity.Property(x => x.ExpectedItemCount).IsRequired();
            entity.Property(x => x.ActualItemCount).IsRequired();
            entity.Property(x => x.Timestamp).IsRequired();
            // entity.Property(x => x.Items).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}