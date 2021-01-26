namespace Services.Core.Events
{
    using System;

    public class OrderItemNotPrepared
    {
        public OrderItemNotPrepared()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public int Status { get; init; }
        
        public DateTime Timestamp { get; }
    }
}