namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class AddExpectedOrderItemContext
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 3)]
        public int Status { get; init; }
    }
}