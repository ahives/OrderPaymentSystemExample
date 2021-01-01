namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record OrderItemExceededPreparationLimit
    {
        public OrderItemExceededPreparationLimit()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<OrderItemExceededPreparationLimit>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public int PreparationCount { get; init; }
        
        public DateTime Timestamp { get; }
    }
}