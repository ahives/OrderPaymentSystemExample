namespace Service.Grpc.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface ICourierDispatcher
    {
        [OperationContract]
        Task<Result<Courier>> Identify(CourierIdentificationContext context);
        
        [OperationContract]
        Task<Result<Courier>> Decline(CourierDispatchContext context);

        [OperationContract]
        Task<Result<Courier>> ChangeStatus(CourierStatusChangeContext context);
        
        [OperationContract]
        Task<Result<Order>> PickUpOrder(CourierDispatchContext context);
        
        [OperationContract]
        Task<Result<Order>> Deliver(CourierDispatchContext context);
    }
}