namespace Service.Grpc.Core
{
    using System.Collections.Generic;
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
        Task<Result<IReadOnlyList<ExpectedOrderItem>>> GetExpectedOrderItems(ExpectedOrderItemContext context);
        
        [OperationContract]
        Task<Result<ExpectedOrderItem>> AddExpectedOrderItem(ExpectedOrderItemContext context);
        
        [OperationContract]
        Task<Result<ExpectedOrderItem>> UpdateExpectedOrderItem(ExpectedOrderItemContext context);
        
        [OperationContract]
        Task<Result<int>> GetIncludedOrderItemCount(OrderItemCountContext context);

        [OperationContract]
        Task<Result<int>> GetExcludedOrderItemCount(OrderItemCountContext context);
        
        [OperationContract]
        Task<Result<Order>> ChangeOrderStatus(CancelOrderContext context);
        
        [OperationContract]
        Task<Result<OrderItem>> ChangeOrderItemStatus(CancelOrderItemContext context);

        [OperationContract]
        Task<Result<ExpectedOrderItem>> ChangeExpectedOrderItemStatus(CancelOrderItemContext context);
    }
}