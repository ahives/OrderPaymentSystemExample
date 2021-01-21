namespace OrderProcessingWbService
{
    using System;

    public record OrderDiscardRequest
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
    }
}