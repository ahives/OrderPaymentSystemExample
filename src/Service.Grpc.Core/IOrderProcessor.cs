namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface IOrderProcessor
    {
        [OperationContract]
        Task<Result<Order>> AddNewOrder(OrderProcessContext context);
        
        [OperationContract]
        Task<Result<OrderItem>> AddNewOrderItem(OrderItemPreparationContext context);
        
        [OperationContract]
        Task<Result<ExpectedOrderItem>> AddExpectedOrderItem(AddExpectedOrderItemContext context);
        
        [OperationContract]
        Task<Result<ExpectedOrderItem>> UpdateExpectedOrderItem(ExpectedOrderItemContext context);
        
        [OperationContract]
        Task<Result<int>> GetExpectedOrderItemCount(ExpectedOrderItemCountContext context);
        
        [OperationContract]
        Task<Result<OrderItem>> CancelOrder(CancelOrderContext context);
    }
}