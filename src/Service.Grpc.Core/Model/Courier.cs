namespace Service.Grpc.Core.Model
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record Courier
    {
        [DataMember(Order = 1)]
        public Guid CourierId { get; init; }
        
        [DataMember(Order = 2)]
        public int Status { get; init; }
        
        [DataMember(Order = 3)]
        public string FirstName { get; init; }
        
        [DataMember(Order = 4)]
        public string LastName { get; init; }
        
        [DataMember(Order = 5)]
        public Address Address { get; init; }
    }
}