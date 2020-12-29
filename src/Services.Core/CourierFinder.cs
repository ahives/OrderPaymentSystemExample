namespace Services.Core
{
    using System.Linq;
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

        public async Task<Result<Courier>> Find(CourierFinderRequest request)
        {
            var target = await (from courier in _db.Couriers
                    from address in _db.Addresses
                    where courier.AddressId == address.AddressId
                        && address.RegionId == request.RegionId
                        && address.City == request.City
                        && courier.IsAvailable
                    select new
                    {
                        Courier = courier,
                        Address = address
                    })
                .FirstOrDefaultAsync();

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            var mapped = MapEntity(target.Courier, target.Address);
            
            target.Courier.IsAvailable = false;
            
            _db.Update(target.Courier);
            
            var changes = await _db.SaveChangesAsync();
            
            return new Result<Courier> {ChangeCount = changes, Value = mapped, IsSuccessful = true};
        }

        Courier MapEntity(CourierEntity courier, AddressEntity address) =>
            new()
            {
                CourierId = courier.CourierId,
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