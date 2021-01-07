namespace Services.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record ShelfManagerRequest
    {
        [DataMember(Order = 1)]
        public Guid OrderItemId { get; init; }
        
        [DataMember(Order = 2)]
        public Guid RestaurantId { get; init; }
        
        [DataMember(Order = 3)]
        public int? ShelfId { get; init; }
        
        [DataMember(Order = 4)]
        public Guid MenuItemId { get; init; }
        
        [DataMember(Order = 5)]
        public int TemperatureId { get; init; }
    }
}