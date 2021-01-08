namespace Service.Grpc.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderManager
    {
        IAsyncEnumerable<Result> Expire();
        // IAsyncEnumerable<Result> Expire();

        // Task<Result> Receive(OrderReceiptConfirmed data);
        
        // Task<Result> Prepare(PrepareOrderItem data);
    }
}