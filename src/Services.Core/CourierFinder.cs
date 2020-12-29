namespace Services.Core
{
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class CourierFinder :
        ICourierFinder
    {
        readonly OrdersDbContext _db;

        public CourierFinder(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Courier>> Find(Address address)
        {
            var target = await _db.Couriers.FirstOrDefaultAsync(x => x.City == address.City
                && x.RegionId == address.RegionId
                && x.IsAvailable);

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            var courier = MapCourier(target);

            target.IsAvailable = false;

            _db.Update(target);

            var changes = await _db.SaveChangesAsync();

            return new Result<Courier> {ChangeCount = changes, Value = courier, IsSuccessful = true};
        }

        Courier MapCourier(CourierEntity courier) =>
            new()
            {
                CourierId = courier.CourierId,
                FirstName = courier.FirstName,
                LastName = courier.LastName,
                Address = new Address
                {
                    Street = courier.Street,
                    City = courier.City,
                    RegionId = courier.RegionId,
                    ZipCode = courier.ZipCode
                }
            };
    }
}