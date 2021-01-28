namespace Services.Core.Model
{
    using System;
    using Data.Core;

    public record Item
    {
        public Guid OrderItemId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public OrderItemStatus Status { get; init; }
        
        public string SpecialInstructions { get; init; }
    }
}