namespace Services.Core
{
    using System;

    public record CourierDispatchCriteria
    {
        public string Street { get; init; }
        
        public string City { get; init; }
        
        public Guid RegionId { get; init; }
        
        public string ZipCode { get; init; }
    }
}