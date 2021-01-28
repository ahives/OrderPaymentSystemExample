namespace OrderProcessingWebService
{
    using System;

    public record OrderDiscardContext
    {
        public Guid OrderId { get; init; }
        
        public Guid OrderItemId { get; init; }
    }
}