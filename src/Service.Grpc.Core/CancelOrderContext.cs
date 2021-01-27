namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;
    using Data.Core;

    [DataContract]
    public record CancelOrderContext
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public OrderStatus Status { get; init; }
    }
}