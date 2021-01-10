namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using Data.Core;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record UpdateCourierStatus
    {
        public UpdateCourierStatus()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<UpdateCourierStatus>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public CourierStatus Status { get; init; }
        
        public DateTime Timestamp { get; }
    }
}