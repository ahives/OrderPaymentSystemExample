namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record TrackOrder
    {
        public TrackOrder()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid? CourierId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}