namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record PrepareOrderItemRequested
    {
        public PrepareOrderItemRequested()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}