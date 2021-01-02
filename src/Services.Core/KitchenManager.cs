namespace Services.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Model;

    public class KitchenManager :
        IKitchenManager
    {
        readonly OrdersDbContext _db;
        readonly List<Shelf> _shelves;
        readonly List<MenuItem> _menuItem;
        readonly List<Restaurant> _restaurants;

        public KitchenManager(OrdersDbContext db)
        {
            _db = db;
            _shelves = GetShelves().ToList();
            _menuItem = GetMenuItems().ToList();
            _restaurants = GetRestaurants().ToList();
        }

        public async Task<Result<Shelf>> MoveToShelf(ShelfMoveCriteria criteria)
        {
            Shelf target = (
                    from menuItem in _menuItem
                    from shelf in _shelves
                    from restaurant in _restaurants
                    where menuItem.MenuItemId == criteria.MenuItemId
                        && menuItem.TemperatureId == shelf.TemperatureId
                        && shelf.RestaurantId == criteria.RestaurantId
                    select shelf)
                .FirstOrDefault();

            if (!IsShelfAvailable(target))
                return new Result<Shelf> {Value = null, IsSuccessful = false};
            
            var orderItem = await _db.OrderItems.FindAsync(criteria.OrderItemId);

            if (orderItem == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            orderItem.ShelfId = target.ShelfId;

            await _db.SaveChangesAsync();

            return new Result<Shelf> {Value = target, IsSuccessful = true};
        }

        public async Task<Result<Shelf>> MoveToOverflow(ShelfMoveCriteria criteria)
        {
            Shelf target = (
                    from shelf in _shelves
                    where shelf.RestaurantId == criteria.RestaurantId
                    select shelf)
                .FirstOrDefault(x => x.IsOverflow);
            
            if (target == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            if (!IsShelfAvailable(target))
                return new Result<Shelf> {Value = null, IsSuccessful = false};
            
            var orderItem = await _db.OrderItems.FindAsync(criteria.OrderItemId);

            if (orderItem == null)
                return new Result<Shelf> {Value = null, IsSuccessful = false};

            orderItem.ShelfId = target.ShelfId;

            await _db.SaveChangesAsync();

            return new Result<Shelf> {Value = target, IsSuccessful = true};
        }

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

        IEnumerable<Restaurant> GetRestaurants() =>
            from restaurant in _db.Restaurants
            select new Restaurant
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                IsActive = restaurant.IsActive,
                IsOpen = restaurant.IsOpen,
                AddressId = restaurant.AddressId
            };

        IEnumerable<MenuItem> GetMenuItems() =>
            from item in _db.MenuItems
            where item.IsActive
            select new MenuItem
            {
                MenuId = item.MenuId,
                MenuItemId = item.MenuItemId,
                Name = item.Name,
                Price = item.Price,
                ShelfLife = item.ShelfLife,
                TemperatureId = item.TemperatureId
            };

        IEnumerable<Shelf> GetShelves() =>
            from shelf in _db.Shelves
            select new Shelf
            {
                ShelfId = shelf.ShelfId,
                RestaurantId = shelf.RestaurantId,
                Name = shelf.Name,
                DecayRate = shelf.DecayRate,
                IsOverflow = shelf.IsOverflow,
                TemperatureId = shelf.TemperatureId,
                Capacity = shelf.Capacity
            };
    }
}