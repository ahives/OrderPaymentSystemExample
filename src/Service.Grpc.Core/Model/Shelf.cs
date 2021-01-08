namespace Service.Grpc.Core.Model
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record Shelf
    {
        [DataMember(Order = 1)]
        public Guid ShelfId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 3)]
        public string Name { get; init; }
        
        [DataMember(Order = 4)]
        public decimal DecayRate { get; init; }

        [DataMember(Order = 5)]
        public bool IsOverflow { get; init; }
        
        [DataMember(Order = 6)]
        public Guid TemperatureId { get; init; }
        
        [DataMember(Order = 7)]
        public int Capacity { get; init; }
    }
}