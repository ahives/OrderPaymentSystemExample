namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;
    using Data.Core;

    [DataContract]
    public record CancelOrderItemContext
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 3)]
        public OrderItemStatus Status { get; init; }
    }
}