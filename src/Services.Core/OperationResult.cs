namespace Services.Core
{
    using System;

    public record OperationResult
    {
        public OperationResult()
        {
            Timestamp = DateTime.Now;
        }

        public int ChangeCount { get; init; }
        
        public OperationType OperationPerformed { get; init; }
        
        public bool IsSuccessful { get; init; }
        
        public DateTime Timestamp { get; }
    }
}