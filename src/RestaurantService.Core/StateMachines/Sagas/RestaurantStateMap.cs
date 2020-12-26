namespace RestaurantService.Core.StateMachines.Sagas
{
    using MassTransit.EntityFrameworkCoreIntegration.Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RestaurantStateMap :
        SagaClassMap<RestaurantState>
    {
        protected override void Configure(EntityTypeBuilder<RestaurantState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
            entity.Property(x => x.OrderId);
            entity.Property(x => x.RestaurantId);
            entity.Property(x => x.Timestamp);
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}