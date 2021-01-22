namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ExpectedOrderItemContext
    {
        [DataMember(Order = 1)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 2)]
        public int Status { get; init; }
    }
}