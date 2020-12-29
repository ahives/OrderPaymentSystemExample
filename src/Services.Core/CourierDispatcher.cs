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

        public async Task<Result<Courier>> Confirm(Guid courierId)
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
            
            target.Courier.Status = CourierStatus.Confirmed;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        public async Task<Result<Courier>> Dispatch(CourierDispatchCriteria request)
        {
            var target = await (
                    from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.AddressId == address.AddressId
                        && address.RegionId == request.RegionId
                        && address.City == request.City
                        && courier.IsActive
                        && courier.Status == CourierStatus.Idle
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            target.Courier.Status = CourierStatus.Dispatched;
            target.Courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

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