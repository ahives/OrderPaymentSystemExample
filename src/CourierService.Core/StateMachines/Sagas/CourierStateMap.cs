namespace CourierService.Core.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CourierStateMap :
        SagaClassMap<CourierState>
    {
        protected override void Configure(EntityTypeBuilder<CourierState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).IsRequired();
            entity.Property(x => x.CustomerId).IsRequired();
            entity.Property(x => x.CourierId);
            entity.Property(x => x.OrderId).IsRequired();
            entity.Property(x => x.RestaurantId).IsRequired();
            entity.Property(x => x.HasCourierArrived).IsRequired();
            entity.Property(x => x.IsOrderReady).IsRequired();
            entity.Property(x => x.OrderCompletionTimeoutTokenId);
            entity.Property(x => x.Timestamp).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}