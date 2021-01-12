namespace Service.Grpc.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record CourierIdentificationRequest
    {
        [DataMember(Order = 1)]
        public Guid CustomerId { get; init; }
    }
}