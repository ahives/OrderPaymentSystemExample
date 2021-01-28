namespace OrderProcessingWebService
{
    using System;

    public record OrderReadyContext
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}