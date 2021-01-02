namespace Services.Core.Model
{
    using System;

    public record Address
    {
        public string Street { get; init; }
        
        public string City { get; init; }
        
        public Guid RegionId { get; init; }
        
        public string ZipCode { get; init; }
    }
}