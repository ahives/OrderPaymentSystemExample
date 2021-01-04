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
        readonly string[] _ingredients;

        public List<RegionEntity> Regions { get; }
        public List<TemperatureEntity> Temperatures { get; }
        public List<RestaurantEntity> Restaurants { get; }
        public List<MenuEntity> Menus { get; }
        public List<MenuItemEntity> MenuItems { get; }
        public List<CustomerEntity> Customers { get; }
        public List<ShelfEntity> Shelves { get; }
        public List<CourierEntity> Couriers { get; }
        public List<OrderEntity> Orders { get; }
        public List<OrderItemEntity> OrderItems { get; }
        public List<AddressEntity> Addresses { get; }
        public List<IngredientEntity> Ingredients { get; }
        public List<InventoryItemEntity> InventoryItems { get; }
        public List<MenuItemIngredientEntity> MenuItemIngredients { get; }

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
            
            _ingredients = new []
            {
                "Chocolate",
                "Ice",
                "American Cheese",
                "Steak",
                "Rice",
                "Dough",
                "Hamburger Patty",
                "Noodles",
                "Chicken",
                "Milk",
                "Orange",
                "Lettuce",
                "Tomato",
                "Lemon",
                "Onion",
                "Potato",
                "Flower Tortilla",
                "Cookie Dough",
                "Bacon",
                "Apple Juice",
                "Hamburger Bun",
                "Egg"
            };

            _zipCodes = new []{"93483", "9230", "83324", "93924", "82474", "69843", "73934"};
            _firstNames = new []{"Albert", "Christy", "Jose", "Stephen", "Michael", "Sarah", "Mia"};
            _lastNames = new []{"Jones", "Lacey", "Jordan", "Curry", "Wiseman", "Chavez", "Williams"};
            _restaurants = new []{"Breakfast & Stuff", "All Day", "Dinner & Things", "Groovy Smoothie", "Big Al's", "Lunch R Us"};
            _menuNames = new []{"Breakfast", "Lunch", "Dinner", "Brunch", "Desert"};
            _temperatures = new []{"Hot", "Cold", "Frozen", "Warm", "Overflow"};
            _regionNames = new []{"California", "New York", "Georgia", "Washington", "Oregon", "Texas"};
            
            Regions = GetRegionFaker().Generate(5);
            Temperatures = GetTemperatureFaker().Generate(4);
            Addresses = GetAddressFaker().Generate(20);
            Restaurants = GetRestaurantFaker().Generate(10);
            Menus = GetMenuFaker().Generate(3);
            MenuItems = GetMenuItemFaker().Generate(20);
            Customers = GetCustomers().Generate(5);
            Ingredients = GetIngredientFaker().Generate(15);
            InventoryItems = GetInventoryItemFaker().Generate(20);
            MenuItemIngredients = GetMenuItemIngredientFaker().Generate(50);

            Shelves = new List<ShelfEntity>();

            for (int i = 0; i < 3; i++)
                Shelves.AddRange(GetShelfFaker(NewId.NextGuid()).Generate(1));
            Shelves.AddRange(GetShelfFaker(NewId.NextGuid(), true).Generate(1));

            Couriers = GetCourierFaker().Generate(20);
            Orders = GetOrderFaker().Generate(10);
            OrderItems = GetOrderItemFaker().Generate(20);
        }

        Faker<AddressEntity> GetAddressFaker()
        {
            var faker = new Faker<AddressEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.AddressId, s => NewId.NextGuid())
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(Regions.Select(x => x.RegionId)))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.CreationTimestamp, DateTime.Now);

            return faker;
        }

        Faker<CourierEntity> GetCourierFaker()
        {
            var faker = new Faker<CourierEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CourierId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 4))
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.IsActive, s => s.PickRandom(true, false))
                .RuleFor(x => x.AddressId, s => s.PickRandom(Addresses.Select(x => x.AddressId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<InventoryItemEntity> GetInventoryItemFaker()
        {
            var faker = new Faker<InventoryItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Ingredient)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.InventoryItemId, s => NewId.NextGuid())
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.IngredientId, s => s.PickRandom(Ingredients.Select(i => i.IngredientId)))
                .RuleFor(x => x.QuantityOnHand, s => s.Random.Decimal(0M, 1000.0M))
                .RuleFor(x => x.ReplenishmentThreshold, s => s.Random.Decimal(5.0M, 10.0M))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<MenuItemIngredientEntity> GetMenuItemIngredientFaker()
        {
            var faker = new Faker<MenuItemIngredientEntity>()
                .StrictMode(true)
                .Ignore(x => x.Ingredient)
                .Ignore(x => x.MenuItem)
                .RuleFor(x => x.MenuItemIngredientId, s => NewId.NextGuid())
                .RuleFor(x => x.MenuItemId, s => s.PickRandom(MenuItems.Select(m => m.MenuItemId)))
                .RuleFor(x => x.IngredientId, s => s.PickRandom(Ingredients.Select(i => i.IngredientId)))
                .RuleFor(x => x.IngredientId, s => s.PickRandom(Ingredients.Select(i => i.IngredientId)))
                .RuleFor(x => x.QuantityToUse, s => s.Random.Decimal(5.0M, 10.0M))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<IngredientEntity> GetIngredientFaker()
        {
            var faker = new Faker<IngredientEntity>()
                .StrictMode(true)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.IngredientId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_ingredients))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(Temperatures.Select(t => t.TemperatureId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<OrderEntity> GetOrderFaker()
        {
            var faker = new Faker<OrderEntity>()
                .StrictMode(true)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Courier)
                .Ignore(x => x.Restaurant)
                .Ignore(x => x.Address)
                .RuleFor(x => x.OrderId, s => NewId.NextGuid())
                .RuleFor(x => x.CustomerPickup, s => false)
                .RuleFor(x => x.Status, s => s.Random.Int(0, 2))
                .RuleFor(x => x.CustomerId, s => s.PickRandom(Customers.Select(c => c.CustomerId)))
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.AddressId, s => s.PickRandom(Addresses.Select(x => x.AddressId)))
                .RuleFor(x => x.CourierId, s => s.PickRandom(Couriers.Select(x => x.CourierId)))
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<CustomerEntity> GetCustomers()
        {
            var faker = new Faker<CustomerEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CustomerId, s => NewId.NextGuid())
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.AddressId, s => s.PickRandom(Addresses.Select(x => x.AddressId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        Faker<RestaurantEntity> GetRestaurantFaker()
        {
            var faker = new Faker<RestaurantEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.RestaurantId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_restaurants))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.IsOpen, s => s.PickRandom(true, false))
                .RuleFor(x => x.AddressId, s => s.PickRandom(Addresses.Select(x => x.AddressId)))
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

        Faker<ShelfEntity> GetShelfFaker(Guid shelfId, bool isOverflow = false)
        {
            var faker = new Faker<ShelfEntity>()
                .StrictMode(true)
                .Ignore(x => x.Temperature)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.ShelfId, s => shelfId)
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.IsOverflow, s => isOverflow)
                .RuleFor(x => x.Capacity, s => s.PickRandom(5, 10, 15, 20))
                .RuleFor(x => x.Name, s => s.Random.Replace("##-????"))
                .RuleFor(x => x.DecayRate, s => s.Random.Decimal(10M, 20M))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(Temperatures.Select(t => t.TemperatureId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }
        
        Faker<MenuItemEntity> GetMenuItemFaker()
        {
            var faker = new Faker<MenuItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Menu)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.MenuItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_menuItems))
                .RuleFor(x => x.Price, s => s.Random.Decimal(1, 25))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.IsActive, s => s.PickRandom(true, false))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(Temperatures.Select(m => m.TemperatureId)))
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

        Faker<TemperatureEntity> GetTemperatureFaker()
        {
            int i = 0;

            var faker = new Faker<TemperatureEntity>()
                .StrictMode(true)
                .RuleFor(x => x.TemperatureId, s => NewId.NextGuid())
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
            int i = 0;
            var faker = new Faker<RegionEntity>()
                .StrictMode(true)
                .RuleFor(x => x.RegionId, s => NewId.NextGuid())
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