namespace Services.Core.Model
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record Order
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 3)]
        public int Status { get; init; }
        
        [DataMember(Order = 4)]
        public DateTime StatusTimestamp { get; init; }
        
        [DataMember(Order = 5)]
        public Guid CustomerId { get; init; }
        
        [DataMember(Order = 6)]
        public Guid CourierId { get; init; }
        
        [DataMember(Order = 7)]
        public Guid AddressId { get; init; }
    }
}