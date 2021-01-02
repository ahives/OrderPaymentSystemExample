namespace Services.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bogus;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class DatabaseTestHarness
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
        protected readonly IServiceCollection _services;
        
        protected ServiceProvider _provider;

        protected List<RegionEntity> Regions { get; set; }
        protected List<TemperatureEntity> Temperatures { get; set; }
        protected List<RestaurantEntity> Restaurants { get; set; }
        protected List<MenuEntity> Menus { get; set; }
        protected List<MenuItemEntity> MenuItems { get; set; }
        protected List<CustomerEntity> Customers { get; set; }
        protected List<ShelfEntity> Shelves { get; set; }
        protected List<CourierEntity> Couriers { get; set; }
        protected List<OrderEntity> Orders { get; set; }
        protected List<OrderItemEntity> OrderItems { get; set; }
        protected List<AddressEntity> Addresses { get; set; }

        public DatabaseTestHarness()
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

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            _services = new ServiceCollection()
                .AddDbContext<OrdersDbContext>(x =>
                    x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")));
        }

        protected Faker<AddressEntity> GetAddressFaker(long addressId, int regionId)
        {
            var faker = new Faker<AddressEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.AddressId, s => addressId++)
                .RuleFor(x => x.City, s => s.PickRandom(_cities))
                .RuleFor(x => x.Street, s => s.PickRandom(_streets))
                .RuleFor(x => x.RegionId, s => regionId)
                .RuleFor(x => x.ZipCode, s => s.PickRandom(_zipCodes))
                .RuleFor(x => x.CreationTimestamp, DateTime.Now);

            return faker;
        }

        protected Faker<CourierEntity> GetCourierFaker(Guid courierId, bool isActive, long addressId)
        {
            var faker = new Faker<CourierEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CourierId, s => courierId)
                .RuleFor(x => x.Status, s => 0)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.IsActive, s => isActive)
                .RuleFor(x => x.AddressId, s => addressId)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<OrderEntity> GetOrderFaker(Guid customerId, Guid restaurantId, long addressId, Guid? courierId)
        {
            var faker = new Faker<OrderEntity>()
                .StrictMode(true)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Courier)
                .Ignore(x => x.Restaurant)
                .Ignore(x => x.Address)
                .RuleFor(x => x.OrderId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 2))
                .RuleFor(x => x.CustomerId, s => customerId)
                .RuleFor(x => x.RestaurantId, s => restaurantId)
                .RuleFor(x => x.AddressId, s => addressId)
                .RuleFor(x => x.CourierId, s => courierId)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<CustomerEntity> GetCustomers(long addressId)
        {
            var faker = new Faker<CustomerEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CustomerId, s => NewId.NextGuid())
                .RuleFor(x => x.FirstName, s => s.PickRandom(_firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(_lastNames))
                .RuleFor(x => x.AddressId, s => addressId)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<RestaurantEntity> GetRestaurantFaker(long addressId)
        {
            var faker = new Faker<RestaurantEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.RestaurantId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_restaurants))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.IsOpen, s => s.PickRandom(true, false))
                .RuleFor(x => x.AddressId, s => addressId)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<OrderItemEntity> GetOrderItemFaker(Guid orderId, int? shelfId, Guid menuItemId)
        {
            var faker = new Faker<OrderItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Order)
                .Ignore(x => x.MenuItem)
                .Ignore(x => x.Shelf)
                .RuleFor(x => x.OrderItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 3))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.MenuItemId, s => menuItemId)
                .RuleFor(x => x.OrderId, s => orderId)
                .RuleFor(x => x.ShelfId, s => shelfId)
                .RuleFor(x => x.TimePrepared, s => DateTime.Now)
                .RuleFor(x => x.ExpiryTimestamp, s => DateTime.Now)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.SpecialInstructions, s => s.PickRandom(string.Empty, "Some random instructions"))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<ShelfEntity> GetShelfFaker(int shelfId, bool isOverflow = false)
        {
            var faker = new Faker<ShelfEntity>()
                .StrictMode(true)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.ShelfId, s => shelfId++)
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(Restaurants.Select(r => r.RestaurantId)))
                .RuleFor(x => x.IsOverflow, s => isOverflow)
                .RuleFor(x => x.Capacity, s => s.PickRandom(5, 10, 15, 20))
                .RuleFor(x => x.Name, s => s.Random.Replace("##-????"))
                .RuleFor(x => x.DecayRate, s => s.Random.Decimal(10M, 20M))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(Temperatures.Select(t => t.TemperatureId)))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<MenuItemEntity> GetMenuItemFaker(int temperatureId, Guid menuId)
        {
            var faker = new Faker<MenuItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Menu)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.MenuItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(_menuItems))
                .RuleFor(x => x.Price, s => s.Random.Decimal(1, 25))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.TemperatureId, s => temperatureId)
                .RuleFor(x => x.MenuId, s => menuId)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<MenuEntity> GetMenuFaker(Guid menuId, Guid restaurantId)
        {
            var faker = new Faker<MenuEntity>()
                .StrictMode(true)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.MenuId, s => menuId)
                .RuleFor(x => x.Name, s => s.PickRandom(_menuNames))
                .RuleFor(x => x.RestaurantId, s => restaurantId)
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected Faker<TemperatureEntity> GetTemperatureFaker(int temperatureId)
        {
            int i = 0;

            var faker = new Faker<TemperatureEntity>()
                .StrictMode(true)
                .RuleFor(x => x.TemperatureId, s => temperatureId)
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

        protected Faker<RegionEntity> GetRegionFaker(int regionId)
        {
            int i = 0;
            var faker = new Faker<RegionEntity>()
                .StrictMode(true)
                .RuleFor(x => x.RegionId, s => regionId)
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