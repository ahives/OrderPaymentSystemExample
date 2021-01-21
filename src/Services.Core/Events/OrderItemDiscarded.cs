namespace Services.Core.Events
{
    using System;

    public record OrderItemDiscarded
    {
        public OrderItemDiscarded()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderItemId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}