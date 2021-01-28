namespace OrderProcessingWebService
{
    using System;

    public record VoidOrderItemRequestContext
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}