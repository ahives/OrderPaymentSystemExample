namespace OrderProcessingWebService
{
    using System;

    public record OrderExpiredContext
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}