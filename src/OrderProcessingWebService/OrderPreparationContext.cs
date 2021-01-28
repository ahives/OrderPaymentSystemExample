namespace OrderProcessingWebService
{
    using System;
    using Services.Core.Model;

    public record OrderPreparationContext
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid AddressId { get; init; }
        
        public Item[] Items { get; init; }
    }
}