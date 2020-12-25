namespace CourierService.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class CourierStateMap :
        SagaClassMap<CourierState>
    {
        protected override void Configure(EntityTypeBuilder<CourierState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
            entity.Property(x => x.CustomerId);
            entity.Property(x => x.OrderId);
            entity.Property(x => x.RestaurantId);
            entity.Property(x => x.Timestamp);
        }
    }
}