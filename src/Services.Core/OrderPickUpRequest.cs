namespace Services.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record OrderPickUpRequest
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 3)]
        public Guid CourierId { get; init; }
    }
}