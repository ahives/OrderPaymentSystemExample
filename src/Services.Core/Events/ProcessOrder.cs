namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record ProcessOrder
    {
        public ProcessOrder()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<ProcessOrder>(x => x.OrderItemId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}