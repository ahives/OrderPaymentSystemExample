namespace Services.Core
{
    using System;

    public class OrderItemStatusPayload :
        OperationPayload
    {
        public Guid OrderId { get; init; }
        
        public int Status { get; init; }
        
        public int ShelfId { get; init; }
    }
}