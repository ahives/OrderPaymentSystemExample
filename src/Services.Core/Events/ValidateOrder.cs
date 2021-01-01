namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;
    using Model;

    public record ValidateOrder
    {
        public ValidateOrder()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<ValidateOrder>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Item[] Items { get; init; }
        
        public DateTime Timestamp { get; }
    }
}