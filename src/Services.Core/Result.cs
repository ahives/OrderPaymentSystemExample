namespace Services.Core
{
    using System;

    public record Result
    {
        public Guid Id { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
}