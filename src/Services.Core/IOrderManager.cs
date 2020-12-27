namespace Services.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Events;

    public interface IOrderManager
    {
        IAsyncEnumerable<OperationResult> Expire();
        // IAsyncEnumerable<Result> Expire();

        Task<OperationResult> Receive(OrderReceiptConfirmed data);
        
        Task<OperationResult> Prepare(PrepareOrderItem data);
    }
}