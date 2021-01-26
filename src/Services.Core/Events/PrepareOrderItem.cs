namespace Services.Core.Events
{
    using System;

    public record PrepareOrderItem
    {
        public PrepareOrderItem()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderItemId { get; init; }
        public Guid OrderId { get; init; }
                
        public Guid RestaurantId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime Timestamp { get; }
    }
}