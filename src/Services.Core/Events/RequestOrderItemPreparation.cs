namespace Services.Core.Events
{
    using System;

    public record RequestOrderItemPreparation
    {
        public RequestOrderItemPreparation()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime Timestamp { get; }
    }
}