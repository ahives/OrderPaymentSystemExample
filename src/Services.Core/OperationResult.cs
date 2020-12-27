namespace Services.Core
{
    using System;

    public record OperationResult
    {
        public int ChangeCount { get; init; }
        
        public OperationType OperationPerformed { get; init; }
        
        public bool IsSuccessful { get; init; }
        
        public DateTime Timestamp { get; init; }
    }

    public enum OperationType
    {
        Save,
        Update,
        Delete
    }
}