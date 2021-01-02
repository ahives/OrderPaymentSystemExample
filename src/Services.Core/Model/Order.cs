namespace Services.Core.Model
{
    using System;

    public record Order
    {
        public Guid OrderId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public int Status { get; init; }
        
        public DateTime StatusTimestamp { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid CourierId { get; init; }
        
        public Guid AddressId { get; init; }
    }
}