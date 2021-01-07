namespace Services.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public record Result
    {
        public Result()
        {
            Timestamp = DateTime.Now;
            Reason = ReasonType.None;
        }

        [DataMember(Order = 1)]
        public int ChangeCount { get; init; }
        
        [DataMember(Order = 2)]
        public ReasonType Reason { get; init; }
        
        [DataMember(Order = 3)]
        public bool IsSuccessful { get; init; }
        
        [DataMember(Order = 4)]
        public DateTime Timestamp { get; }
    }

    [DataContract]
    public record Result<T>
    {
        public Result()
        {
            Timestamp = DateTime.Now;
            Reason = ReasonType.None;
        }

        [DataMember(Order = 1)]
        public T Value { get; init; }
        
        [DataMember(Order = 2)]
        public int ChangeCount { get; init; }
        
        [DataMember(Order = 3)]
        public ReasonType Reason { get; init; }
        
        [DataMember(Order = 4)]
        public bool IsSuccessful { get; init; }
        
        [DataMember(Order = 5)]
        public DateTime Timestamp { get; }
    }
}