namespace Services.Core
{
    using System.Threading.Tasks;

    public interface IOrderManager
    {
        // IAsyncEnumerable<Result> Expire();

        Task<OperationResult> Save(SaveContext<OrderPayload> context);
    }
}