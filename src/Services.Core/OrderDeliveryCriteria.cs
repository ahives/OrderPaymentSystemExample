namespace Services.Core
{
    using System;

    public record OrderDeliveryCriteria
    {
        public Guid OrderId { get; init; }
        
        public Guid CourierId { get; init; }
    }
}