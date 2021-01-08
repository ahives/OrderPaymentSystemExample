namespace Service.Grpc.Core.Model
{
    using System;

    public record Restaurant
    {
        public Guid RestaurantId { get; init; }
        
        public string Name { get; init; }
        
        public bool IsOpen { get; init; }
        
        public bool IsActive { get; init; }
        
        public Guid AddressId { get; init; }
    }
}