namespace DatabaseSeederConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bogus;
    using Data.Core.Model;
    using MassTransit;

    public class OrdersDataGenerator :
        IOrdersDataGenerator
    {
        readonly string[] _cities;
        readonly string[] _streets;
        readonly string[] _zipCodes;
        readonly string[] _firstNames;
        readonly string[] _lastNames;
        readonly string[] _restaurants;
        readonly string[] _menuItems;
        readonly string[] _menuNames;
        readonly string[] _temperatures;
        readonly string[] _regionNames;
        
        public List<RegionEntity> Regions { get; }
        public List<StorageTemperatureEntity> Temperatures { get; }
        public List<RestaurantEntity> Restaurants { get; }
        public List<MenuEntity> Menus { get; }
        public List<MenuItemEntity> MenuItems { get; }
        public List<CustomerEntity> Customers { get; }
        public List<ShelfEntity> Shelves { get; }
        public List<CourierEntity> Couriers { get; }
        public List<OrderEntity> Orders { get; }
        public List<OrderItemEntity> OrderItems { get; }

        public OrdersDataGenerator()
        {
            _cities = new[]
            {
                "Oakland",
                "Atlanta",
                "Seattle",
                "Portland",
                "Los Angeles",
                "San Francisco",
                "New York",
                "Chicago",
                "New Orleans",
                "Philadelphia"
            };
            
            _streets = new []
            {
                "123 E. 12th St.",
                "425 Broadway Ave.",
                "948 West St.",
                "99 California St.",
                "123 E. 14th Blvd.",
                "939 Crenshaw Blvd.",
                "294 E. Cotati Blvd."
            };
            
            _menuItems = new []
            {
                "Hot Chocolate",
                "Smoothie",
                "Cheese Pizza",
                "Steak",
                "Rice and Gravy",
                "Philly Cheese Steak",
                "Hamburger",
                "Lasagna",
                "Soup",
                "Milk",
                "Orange Juice",
                "House Salad",
                "Fruit Punch",
                "Mac & Cheese",
                "Ice Cream",
                "French Fries",
                "Bacon, Egg, and Cheese Burrito",
                "Desert"
            };

            _zipCodes = new []{"93483", "9230", "83324", "93924", "82474", "69843", "73934"};
            _firstNames = new []{"Albert", "Christy", "Jose", "Stephen", "Michael", "Sarah", "Mia"};
            _lastNames = new []{"Jones", "Lacey", "Jordan", "Curry", "Wiseman", "Chavez", "Williams"};
            _restaurants = new []{"Breakfast & Stuff", "All Day", "Dinner & Things", "Groovy Smoothie", "Big Al's", "Lunch R Us"};
            _menuNames = new []{"Breakfast", "Lunch", "Dinner", "Brunch", "Desert"};
            _temperatures = new []{"Hot", "Cold", "Frozen", "Warm", "Overflow"};
            _regionNames = new []{"California", "New York", "Georgia", "Washington", "Oregon", "Texas"};
            
            Regions = GetRegionFaker().Generate(3);
            Temperatures = GetStorageTemperatureFaker().Generate(4);
            Restaurants = GetRestaurantFaker().Generate(5);
            Menus = GetMenuFaker().Generate(3);
            MenuItems = GetMenuItemFaker().Generate(15);
            Customers = GetCustomers().Generate(5);
            Shelves = GetShelfFaker().Generate(4);
            Couriers = GetCourierFaker().Generate(5);
            Orders = GetOrderFaker().Generate(3);
            OrderItems = GetOrderItemFaker().Generate(20);
        }
        
        Faker<CourierEntity> GetCourierFaker()
        {
            var faker = new Faker<CourierEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.CourierId, s => NewId.NextGuid())
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.IsAvailable, s => s.PickRandom(true, false))
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(Regions.Select(x => x.RegionId)))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<OrderEntity> GetOrderFaker()
        {
            var faker = new Faker<OrderEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Courier)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.OrderId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 2))
                .RuleFor(x => x.CustomerId, s => s.PickRandom(Customers.Select(c => c.CustomerId)))
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(Regions.Select(x => x.RegionId)))
                .RuleFor(x => x.CourierId, s => s.PickRandom(Couriers.Select(x => x.CourierId)))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<CustomerEntity> GetCustomers()
        {
            var faker = new Faker<CustomerEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.CustomerId, s => NewId.NextGuid())
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(Regions.Select(x => x.RegionId)))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<RestaurantEntity> GetRestaurantFaker()
        {
            var faker = new Faker<RestaurantEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.RestaurantId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_restaurants))
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(Regions.Select(x => x.RegionId)))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<OrderItemEntity> GetOrderItemFaker()
        {
            var faker = new Faker<OrderItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Order)
                .Ignore(x => x.MenuItem)
                .Ignore(x => x.Shelf)
                .RuleFor(x => x.OrderItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 3))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.MenuItemId, s => s.PickRandom(MenuItems.Select(m => m.MenuItemId)))
                .RuleFor(x => x.OrderId, s => s.PickRandom(Orders.Select(m => m.OrderId)))
                .RuleFor(x => x.ShelfId, s => s.PickRandom(Shelves.Select(m => m.ShelfId)))
                .RuleFor(x => x.TimePrepared, s => DateTime.Now)
                .RuleFor(x => x.ExpiryTimestamp, s => DateTime.Now)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.SpecialInstructions, s => s.PickRandom(string.Empty, "Some random instructions"))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<ShelfEntity> GetShelfFaker()
        {
            int shelfId = 1;
            
            var faker = new Faker<ShelfEntity>()
                .StrictMode(true)
                .Ignore(x => x.StorageTemperature)
                .RuleFor(x => x.ShelfId, s => shelfId++)
                .RuleFor(x => x.Capacity, s => s.PickRandom(5, 10, 15, 20))
                .RuleFor(x => x.Name, s => s.Random.Replace("##-????"))
                .RuleFor(x => x.DecayRate, s => s.Random.Decimal(10M, 20M))
                .RuleFor(x => x.StorageTemperatureId, s => s.PickRandom(Temperatures.Select(t => t.StorageTemperatureId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }
        
        Faker<MenuItemEntity> GetMenuItemFaker()
        {
            var faker = new Faker<MenuItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Menu)
                .Ignore(x => x.StorageTemperature)
                .RuleFor(x => x.MenuItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_menuItems))
                .RuleFor(x => x.Price, s => s.Random.Decimal(1, 25))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.IsValid, s => s.PickRandom(true, false))
                .RuleFor(x => x.StorageTemperatureId, s => s.PickRandom(Temperatures.Select(m => m.StorageTemperatureId)))
                .RuleFor(x => x.MenuId, s => s.PickRandom(Menus.Select(m => m.MenuId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }
        
        Faker<MenuEntity> GetMenuFaker()
        {
            var faker = new Faker<MenuEntity>()
                .StrictMode(true)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.MenuId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_menuNames))
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.IsActive, s => s.PickRandom(true, false))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<StorageTemperatureEntity> GetStorageTemperatureFaker()
        {
            int storageTemperatureId = 1;
            int i = 0;

            var faker = new Faker<StorageTemperatureEntity>()
                .StrictMode(true)
                .RuleFor(x => x.StorageTemperatureId, s => storageTemperatureId++)
                .RuleFor(x => x.Name, s =>
                {
                    if (i < _temperatures.Length)
                        return _temperatures[i++];

                    i++;
                    return null;
                })
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<RegionEntity> GetRegionFaker()
        {
            int regionId = 1;

            int i = 0;
            var faker = new Faker<RegionEntity>()
                .StrictMode(true)
                .RuleFor(x => x.RegionId, s => regionId++)
                .RuleFor(x => x.Name, s =>
                {
                    if (i < _regionNames.Length)
                        return _regionNames[i++];

                    i++;
                    return null;
                })
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }
    }
}