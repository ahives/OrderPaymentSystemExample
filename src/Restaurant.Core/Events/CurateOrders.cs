namespace Restaurant.Core.Events
{
    using System;

    public record CurateOrders
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
}