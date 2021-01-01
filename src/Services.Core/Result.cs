namespace Services.Core
{
    using System;
    using Model;

    public record Result
    {
        public Result()
        {
            Timestamp = DateTime.Now;
            Reason = ReasonType.None;
        }

        public int ChangeCount { get; init; }
        
        public ReasonType Reason { get; init; }
        
        public bool IsSuccessful { get; init; }
        
        public DateTime Timestamp { get; }
    }

    public record Result<T>
    {
        public Result()
        {
            Timestamp = DateTime.Now;
            Reason = ReasonType.None;
        }

        public T Value { get; init; }
        
        public int ChangeCount { get; init; }
        
        public ReasonType Reason { get; init; }
        
        public bool IsSuccessful { get; init; }
        
        public DateTime Timestamp { get; }
    }
}