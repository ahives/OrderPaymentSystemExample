namespace Services.Core.Events
{
    using System;
    using System.Runtime.CompilerServices;
    using MassTransit;
    using MassTransit.Topology.Topologies;

    public record PrepareOrderItem
    {
        public PrepareOrderItem()
        {
            EventId = NewId.NextGuid();
            Timestamp = DateTime.Now;
        }

        [ModuleInitializer]
        internal static void Init()
        {
            GlobalTopology.Send.UseCorrelationId<PrepareOrderItem>(x => x.OrderId);
        }

        public Guid EventId { get; }
        
        public Guid OrderId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public string SpecialInstructions { get; init; }
        
        public DateTime Timestamp { get; }
    }
}