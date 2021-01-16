namespace OrderProcessingWebService
{
    using System;
    using Services.Core.Model;

    public record PrepareOrderRequest
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Item[] Items { get; init; }
    }
}