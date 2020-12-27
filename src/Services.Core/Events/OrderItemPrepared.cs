namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record OrderItemPrepared
    {
        public OrderItemPrepared()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}