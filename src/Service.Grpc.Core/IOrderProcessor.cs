namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface IOrderProcessor
    {
        [OperationContract]
        Task<Result<OrderItem>> PrepareItem(OrderPrepRequest request);
    }
}