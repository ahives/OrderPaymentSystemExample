namespace OrderProcessingWbService
{
    using System;

    public record OrderExpiredRequest
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}