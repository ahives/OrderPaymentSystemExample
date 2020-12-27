namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record OrderItemExpired
    {
        public OrderItemExpired()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}