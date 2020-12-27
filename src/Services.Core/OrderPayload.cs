namespace Services.Core
{
    using System;
    using Model;

    public record OrderPayload :
        OperationPayload
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid? CourierId { get; init; }
        
        public string Street { get; init; }
        
        public string City { get; init; }
        
        public int RegionId { get; init; }
        
        public string ZipCode { get; init; }
        
        public Item[] Items { get; init; }
    }
}