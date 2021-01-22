namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record CourierIdentificationContext
    {
        [DataMember(Order = 1)]
        public Guid CustomerId { get; init; }
    }
}