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

        public async Task<Result<Courier>> Confirm(CourierStatusChangeRequest request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == request.CourierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            
            if (!target.Courier.IsActive)
                return new Result<Courier> {Reason = ReasonType.CourierNotActive, IsSuccessful = false};
            
            if (target.Courier.Status != (int)CourierStatus.Idle)
                return new Result<Courier> {Reason = ReasonType.CourierNotAvailable, IsSuccessful = false};

            target.Courier.Status = (int)CourierStatus.Confirmed;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
                return new Result<Courier> {ChangeCount = changes, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Courier>> Decline(CourierStatusChangeRequest request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == request.CourierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};

            target.Courier.Status = (int)CourierStatus.Declined;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
                return new Result<Courier> {ChangeCount = changes, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Courier>> EnRoute(CourierStatusChangeRequest request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == request.CourierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};

            target.Courier.Status = (int)request.Status;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
                return new Result<Courier> {ChangeCount = changes, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Order>> PickUpOrder(OrderPickUpRequest request)
        {
            var restaurant = await _db.Restaurants.FindAsync(request.RestaurantId);
            
            if (restaurant == null || !restaurant.IsActive)
                return new Result<Order> {Reason = ReasonType.RestaurantNotActive, IsSuccessful = false};
            
            if (!restaurant.IsOpen)
                return new Result<Order> {Reason = ReasonType.RestaurantNotOpen, IsSuccessful = false};

            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == request.CourierId
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
            
            var order = await _db.Orders.FindAsync(request.OrderId);
            
            if (order == null)
                return new Result<Order> {ChangeCount = 0, IsSuccessful = false};

            order.CourierId = target.Courier.CourierId;
            order.Status = (int) OrderStatus.Delivering;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
                return new Result<Order> {ChangeCount = changes, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, order);
            
            return new Result<Order> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Order>> Deliver(OrderDeliveryRequest request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.CourierId == request.CourierId
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Order> {ChangeCount = 0, IsSuccessful = false};
            
            target.Courier.Status = (int)CourierStatus.DeliveredOrder;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var order = await _db.Orders.FindAsync(request.OrderId);
            
            if (order == null)
                return new Result<Order> {ChangeCount = 0, IsSuccessful = false};

            order.CourierId ??= target.Courier.CourierId;
            order.Status = (int) OrderStatus.Delivered;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
                return new Result<Order> {ChangeCount = changes, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, order);
            
            return new Result<Order> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Courier>> Dispatch(CourierDispatchRequest request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.AddressId == address.AddressId
                        && address.RegionId == request.RegionId
                        && address.City == request.City
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
            
            if (changes <= 0)
                return new Result<Courier> {ChangeCount = changes, IsSuccessful = false};

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