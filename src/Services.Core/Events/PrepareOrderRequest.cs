namespace Services.Core.Events
{
    using System;
    using MassTransit;
    using Model;

    public record PrepareOrderRequest
    {
        public PrepareOrderRequest()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        // [ModuleInitializer]
        // internal static void Init()
        // {
        //     GlobalTopology.Send.UseCorrelationId<PrepareOrder>(x => x.OrderId);
        // }

        public Guid EventId { get; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public Guid AddressId { get; init; }
        
        public Item[] Items { get; init; }
        
        public DateTime Timestamp { get; }
    }
}