namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface IOrderProcessor
    {
        [OperationContract]
        Task<Result<Order>> AddNewOrder(OrderProcessRequest request);
        
        [OperationContract]
        Task<Result<OrderItem>> AddNewOrderItem(OrderPrepRequest request);
    }
}