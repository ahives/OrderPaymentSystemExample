namespace Services.Core.Events
{
    using System;

    public record StorageCapacityExceeded
    {
        public Guid OrderId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid ShelfId { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
}