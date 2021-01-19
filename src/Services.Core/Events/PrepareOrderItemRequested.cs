namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record PrepareOrderItemRequested
    {
        public PrepareOrderItemRequested()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<PrepareOrderItemRequested>(x => x.OrderItemId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        public Guid OrderItemId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime Timestamp { get; }
    }
}