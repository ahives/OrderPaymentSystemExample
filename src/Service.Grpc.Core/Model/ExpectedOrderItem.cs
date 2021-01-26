namespace Service.Grpc.Core.Model
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ExpectedOrderItem
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 3)]
        public int Status { get; init; }
    }
}