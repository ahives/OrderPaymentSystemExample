namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface IPrepareOrder
    {
        [OperationContract]
        Task<Result<OrderItem>> Prepare(OrderPrepRequest request);
    }
}