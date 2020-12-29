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

        public async Task<Result<Courier>> Find(CourierFinderRequest request)
        {
            var target = await _db.Couriers.FirstOrDefaultAsync(x => x.City == request.City
                && x.RegionId == request.RegionId
                && x.IsAvailable);

            if (target == null)
                return new Result<Courier> {ChangeCount = 0, IsSuccessful = false};
            
            var courier = MapEntity(target);

            target.IsAvailable = false;

            _db.Update(target);

            var changes = await _db.SaveChangesAsync();

            return new Result<Courier> {ChangeCount = changes, Value = courier, IsSuccessful = true};
        }

        Courier MapEntity(CourierEntity entity) =>
            new()
            {
                CourierId = entity.CourierId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Address = new Address
                {
                    Street = entity.Street,
                    City = entity.City,
                    RegionId = entity.RegionId,
                    ZipCode = entity.ZipCode
                }
            };
    }
}