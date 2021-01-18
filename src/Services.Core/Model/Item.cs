namespace Services.Core.Model
{
    using System;

    public record Item
    {
        public Guid OrderItemId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public int Status { get; init; }
        
        public string SpecialInstructions { get; init; }
    }
}