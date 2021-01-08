namespace Service.Grpc.Core.Model
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record OrderItem
    {
        [DataMember(Order = 1)]
        public Guid OrderId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 3)]
        public int Status { get; init; }
        
        [DataMember(Order = 4)]
        public DateTime StatusTimestamp { get; init; }
        
        [DataMember(Order = 5)]
        public Guid MenuItemId { get; init; }
        
        [DataMember(Order = 6)]
        public Guid? ShelfId { get; init; }
        
        [DataMember(Order = 7)]
        public string SpecialInstructions { get; init; }
        
        [DataMember(Order = 8)]
        public DateTime? TimePrepared { get; init; }
        
        [DataMember(Order = 9)]
        public DateTime? ExpiryTimestamp { get; init; }
        
        [DataMember(Order = 10)]
        public decimal ShelfLife { get; init; }
        
        [DataMember(Order = 11)]
        public DateTime CreationTimestamp { get; init; }
    }
}