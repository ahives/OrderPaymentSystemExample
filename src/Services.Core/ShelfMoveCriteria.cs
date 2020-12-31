namespace Services.Core
{
    using System;

    public record ShelfMoveCriteria
    {
        public Guid OrderItemId { get; init; }
        
        public int? ShelfId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public int TemperatureId { get; init; }
    }
}