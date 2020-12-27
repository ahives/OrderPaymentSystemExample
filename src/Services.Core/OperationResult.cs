namespace Services.Core
{
    using System;
    using Model;

    public record OperationResult
    {
        public OperationResult()
        {
            Timestamp = DateTime.Now;
        }

        public PreparedOrderItem OrderItem { get; init; }
        
        public Shelf Shelf { get; init; }
        
        public int ChangeCount { get; init; }
        
        public OperationType OperationPerformed { get; init; }
        
        public bool IsSuccessful { get; init; }
        
        public DateTime Timestamp { get; }
    }
}