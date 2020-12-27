namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record OrderItemExceededPreparationLimit
    {
        public OrderItemExceededPreparationLimit()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public int PreparationCount { get; init; }
        
        public DateTime Timestamp { get; }
    }
}