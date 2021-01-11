namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface ICourierDispatcher
    {
        [OperationContract]
        Task<Result<Courier>> Confirm(CourierDispatchRequest request);
        
        [OperationContract]
        Task<Result<Courier>> Decline(CourierDispatchRequest request);

        [OperationContract]
        Task<Result<Courier>> ChangeStatus(CourierStatusChangeRequest request);
        
        [OperationContract]
        Task<Result<Order>> PickUpOrder(CourierDispatchRequest request);
        
        [OperationContract]
        Task<Result<Order>> Deliver(CourierDispatchRequest request);
        
        [OperationContract]
        Task<Result<Courier>> Dispatch(CourierDispatchRequest request);
    }
}