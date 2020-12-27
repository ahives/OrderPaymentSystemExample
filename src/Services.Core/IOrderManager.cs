namespace Services.Core
{
    using System.Threading.Tasks;
    using Events;

    public interface IOrderManager
    {
        // IAsyncEnumerable<Result> Expire();

        Task<OperationResult> Receive(OrderReceived data);
        
        Task<OperationResult> Prepare(PrepareOrderItem data);
    }
}