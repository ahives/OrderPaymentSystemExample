namespace Services.Core
{
    public record SaveContext<TPayload>
        where TPayload : class, OperationPayload, new()
    {
        public TPayload Payload { get; init; }
    }
}