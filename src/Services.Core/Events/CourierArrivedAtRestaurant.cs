namespace Services.Core.Events
{
    using System;

    public record CourierArrivedAtRestaurant
    {
        public CourierArrivedAtRestaurant()
        {
            Timestamp = DateTime.Now;
        }

        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}