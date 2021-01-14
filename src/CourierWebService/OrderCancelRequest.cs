namespace CourierWebService
{
    using System;

    public record OrderCancelRequest
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}