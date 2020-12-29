namespace Services.Core
{
    using System.Threading.Tasks;
    using Model;

    public interface ICook
    {
        Task<Result<OrderItem>> Prepare(OrderPrepRequest request);
    }
}