namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record CancelOrderContext
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
    }
}