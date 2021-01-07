namespace Services.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface ICourierDispatcher
    {
        [OperationContract]
        Task<Result<Courier>> Confirm(CourierStatusChangeRequest request);
        
        [OperationContract]
        Task<Result<Courier>> Decline(CourierStatusChangeRequest request);
        
        [OperationContract]
        Task<Result<Courier>> EnRoute(CourierStatusChangeRequest request);
        
        [OperationContract]
        Task<Result<Order>> PickUpOrder(OrderPickUpRequest request);
        
        [OperationContract]
        Task<Result<Order>> Deliver(OrderDeliveryRequest request);
        
        [OperationContract]
        Task<Result<Courier>> Dispatch(CourierDispatchRequest request);
    }
}