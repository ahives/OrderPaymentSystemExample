namespace Services.Core
{
    public record CourierDispatchCriteria
    {
        public string Street { get; init; }
        
        public string City { get; init; }
        
        public int RegionId { get; init; }
        
        public string ZipCode { get; init; }
    }
}