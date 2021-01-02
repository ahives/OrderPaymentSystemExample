namespace Services.Core.Model
{
    using System;

    public record OrderItem
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public int Status { get; init; }
        
        public DateTime StatusTimestamp { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public Guid? ShelfId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime? TimePrepared { get; init; }
        
        public DateTime? ExpiryTimestamp { get; init; }
        
        public decimal ShelfLife { get; init; }
        
        public DateTime CreationTimestamp { get; init; }
    }
}