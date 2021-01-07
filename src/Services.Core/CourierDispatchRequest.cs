namespace Services.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record CourierDispatchRequest
    {
        [DataMember(Order = 1)]
        public string Street { get; init; }
        
        [DataMember(Order = 2)]
        public string City { get; init; }
        
        [DataMember(Order = 3)]
        public Guid RegionId { get; init; }
        
        [DataMember(Order = 4)]
        public string ZipCode { get; init; }
    }
}