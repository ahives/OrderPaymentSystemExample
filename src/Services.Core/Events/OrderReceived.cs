namespace Services.Core.Events
{
    using System;
    using MassTransit;
    using Model;

    public record OrderReceived
    {
        public OrderReceived()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public string Street { get; init; }
        
        public string City { get; init; }
        
        public int RegionId { get; init; }
        
        public string ZipCode { get; init; }
        
        public Item[] Items { get; init; }
        
        public DateTime Timestamp { get; }
    }
}