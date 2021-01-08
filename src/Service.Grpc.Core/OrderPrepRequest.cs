namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record OrderPrepRequest
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 3)]
        public Guid MenuItemId { get; init; }
        
        [DataMember(Order = 4)]
        public string SpecialInstructions { get; init; }
    }
}