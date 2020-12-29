namespace Services.Core
{
    using System;

    public record OrderPickUpCriteria
    {
        public Guid OrderId { get; init; }
        
        public Guid CourierId { get; init; }
    }
}