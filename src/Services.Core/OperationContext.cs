namespace Services.Core
{
    public record OperationContext<TPayload>
        where TPayload : class, OperationPayload, new()
    {
        public TPayload Payload { get; init; }
    }
}