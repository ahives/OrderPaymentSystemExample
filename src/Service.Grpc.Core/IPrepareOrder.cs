namespace Service.Grpc.Core
{
    using System.Threading.Tasks;
    using Model;

    public interface IPrepareOrder
    {
        Task<Result<OrderItem>> Prepare(OrderPrepCriteria criteria);
    }
}