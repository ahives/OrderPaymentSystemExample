namespace Services.Core.Events
{
    using System;
    using MassTransit;

    public record PrepareOrderItem
    {
        public PrepareOrderItem()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
        
        public int Status { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime Timestamp { get; }
    }
}