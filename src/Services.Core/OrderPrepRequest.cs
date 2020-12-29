namespace Services.Core
{
    using System;

    public record OrderPrepRequest
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
    }
}