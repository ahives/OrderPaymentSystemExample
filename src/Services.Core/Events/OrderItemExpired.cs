namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record OrderItemExpired
    {
        public OrderItemExpired()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<OrderItemExpired>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}