namespace Services.Core.Events
{
    using System;

    public record CourierNotDispatched
    {
        public CourierNotDispatched()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderId { get; init; }
        
        public Guid CourierId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}