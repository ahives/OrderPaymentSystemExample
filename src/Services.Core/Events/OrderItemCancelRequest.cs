namespace Services.Core.Events
{
    using System;

    public record OrderItemCancelRequest
    {
        public OrderItemCancelRequest()
        {
            Timestamp = DateTime.Now;
        }

        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}