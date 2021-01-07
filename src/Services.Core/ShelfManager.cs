namespace Services.Core
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Model;

    public class ShelfManager :
        IShelfManager
    {
        readonly OrdersDbContext _db;

        public ShelfManager(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Shelf>> MoveToShelf(ShelfManagerRequest request)
        {
            Shelf target = (
                    from menuItem in _db.MenuItems
                    from shelf in _db.Shelves
                    from restaurant in _db.Restaurants
                    where menuItem.MenuItemId == request.MenuItemId
                        && menuItem.TemperatureId == shelf.TemperatureId
                        && shelf.RestaurantId == request.RestaurantId
                    select MapEntity(shelf))
                .FirstOrDefault();

            if (!IsShelfAvailable(target))
                return new Result<Shelf> {Value = null, IsSuccessful = false};
            
            var orderItem = await _db.OrderItems.FindAsync(request.OrderItemId);

            if (orderItem == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            orderItem.ShelfId = target.ShelfId;

            await _db.SaveChangesAsync();

            return new Result<Shelf> {Value = target, IsSuccessful = true};
        }

        public async Task<Result<Shelf>> MoveToOverflow(ShelfManagerRequest request)
        {
            Shelf target = (
                    from shelf in _db.Shelves
                    where shelf.RestaurantId == request.RestaurantId
                    select MapEntity(shelf))
                .FirstOrDefault(x => x.IsOverflow);
            
            if (target == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            if (!IsShelfAvailable(target))
                return new Result<Shelf> {Value = null, IsSuccessful = false};
            
            var orderItem = await _db.OrderItems.FindAsync(request.OrderItemId);

            if (orderItem == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            orderItem.ShelfId = target.ShelfId;

            await _db.SaveChangesAsync();

            return new Result<Shelf> {Value = target, IsSuccessful = true};
        }

        Shelf MapEntity(ShelfEntity entity) =>
            new()
            {
                ShelfId = entity.ShelfId,
                RestaurantId = entity.RestaurantId,
                Name = entity.Name,
                DecayRate = entity.DecayRate,
                IsOverflow = entity.IsOverflow,
                TemperatureId = entity.TemperatureId,
                Capacity = entity.Capacity
            };

        bool IsShelfAvailable(Shelf shelf)
        {
            if (shelf == null)
                return false;
            
            var orderItems =
                from orderItem in _db.OrderItems
                where orderItem.Status == (int)OrderItemStatus.Prepared && orderItem.ShelfId == shelf.ShelfId
                select orderItem;

            return orderItems.Count() < shelf.Capacity;
        }
    }
}