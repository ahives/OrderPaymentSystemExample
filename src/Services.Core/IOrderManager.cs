namespace Services.Core
{
    using System.Threading.Tasks;

    public interface IOrderManager
    {
        // IAsyncEnumerable<Result> Expire();

        Task<OperationResult> Save(OperationContext<OrderPayload> context);
        
        Task<OperationResult> Prepare(OperationContext<OrderItemStatusPayload> context);
    }
}