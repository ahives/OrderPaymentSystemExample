namespace Services.Core.Events
{
    using System;
    using Model;

    public record OrderValidated
    {
        public OrderValidated()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Item[] Items { get; init; }
        
        public DateTime Timestamp { get; }
    }
}