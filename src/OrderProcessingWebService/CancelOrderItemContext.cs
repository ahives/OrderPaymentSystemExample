namespace OrderProcessingWbService
{
    using System;

    public record CancelOrderItemContext
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
    }
}