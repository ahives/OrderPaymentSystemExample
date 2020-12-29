namespace Services.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class CourierDispatcher :
        ICourierDispatcher
    {
        readonly OrdersDbContext _db;

        public CourierDispatcher(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Courier>> Confirm(Guid courierId) => await ChangeCourierStatus(courierId, CourierStatus.Confirmed);

        public async Task<Result<Order>> PickUpOrder(OrderPickUpCriteria criteria)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == criteria.CourierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Order> {ChangeCount = 0, IsSuccessful = false};
            
            target.Courier.Status = (int)CourierStatus.PickedUpOrder;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            // TODO: modify order status
            var order = await _db.Orders.FindAsync(criteria.OrderId);
            
            if (order == null)
                return new Result<Order> {ChangeCount = 0, IsSuccessful = false};

            order.CourierId = target.Courier.CourierId;
            order.Status = (int) OrderStatus.Delivering;

            _db.Update(order);
            
            var changes = await _db.SaveChangesAsync();
            
            var mapped = MapEntity(target.Courier, order);
            
            return new Result<Order> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Courier>> Deliver(Guid courierId) => await ChangeCourierStatus(courierId, CourierStatus.DeliveredOrder);

        public async Task<Result<Courier>> Dispatch(CourierDispatchCriteria criteria)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.AddressId == address.AddressId
                        && address.RegionId == criteria.RegionId
                        && address.City == criteria.City
                        && courier.IsActive
                        && courier.Status == (int)CourierStatus.Idle
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            target.Courier.Status = (int)CourierStatus.Dispatched;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        async Task<Result<Courier>> ChangeCourierStatus(Guid courierId, CourierStatus status)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == courierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            target.Courier.Status = (int)status;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        Order MapEntity(CourierEntity courier, OrderEntity order) =>
            new()
            {
                OrderId = order.OrderId,
                Status = order.Status,
                StatusTimestamp = order.StatusTimestamp,
                RestaurantId = order.RestaurantId,
                CustomerId = order.CustomerId,
                CourierId = courier.CourierId,
                AddressId = order.AddressId
            };

        Courier MapEntity(CourierEntity courier, AddressEntity address) =>
            new()
            {
                CourierId = courier.CourierId,
                Status = courier.Status,
                FirstName = courier.FirstName,
                LastName = courier.LastName,
                Address = new Address
                {
                    Street = address.Street,
                    City = address.City,
                    RegionId = address.RegionId,
                    ZipCode = address.ZipCode
                }
            };
    }
}