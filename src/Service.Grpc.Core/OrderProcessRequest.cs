namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    public class OrderProcessRequest
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 3)]
        public Guid CustomerId { get; init; }
        
        [DataMember(Order = 4)]
        public Guid AddressId { get; init; }
    }
}