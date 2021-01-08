namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;
    using Data.Core;

    [DataContract]
    public record CourierDispatchRequest
    {
        [DataMember(Order = 1)]
        public Guid CourierId { get; init; }

        [DataMember(Order = 2)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 3)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 4)]
        public CourierStatus Status { get; init; }

        [DataMember(Order = 5)]
        public string Street { get; init; }
        
        [DataMember(Order = 6)]
        public string City { get; init; }
        
        [DataMember(Order = 7)]
        public Guid RegionId { get; init; }
        
        [DataMember(Order = 8)]
        public string ZipCode { get; init; }
    }
}