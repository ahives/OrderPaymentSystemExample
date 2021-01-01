namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record StorageCapacityExceeded
    {
        public StorageCapacityExceeded()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<StorageCapacityExceeded>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid ShelfId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}