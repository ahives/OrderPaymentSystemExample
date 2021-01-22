namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;
    using Data.Core;

    [DataContract]
    public record CourierStatusChangeContext
    {
        [DataMember(Order = 1)]
        public Guid CourierId { get; init; }
        
        [DataMember(Order = 2)]
        public CourierStatus Status { get; init; }
    }
}