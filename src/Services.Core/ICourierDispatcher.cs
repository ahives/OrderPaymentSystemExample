namespace Services.Core
{
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface ICourierDispatcher
    {
        Task<Result<Courier>> Confirm(Guid courierId);
        
        Task<Result<Order>> PickUpOrder(OrderPickUpCriteria criteria);
        
        Task<Result<Courier>> Deliver(Guid courierId);
        
        Task<Result<Courier>> Dispatch(CourierDispatchCriteria criteria);
    }
}