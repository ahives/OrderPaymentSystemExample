namespace Services.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Bogus;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Model;
    using NUnit.Framework;

    [TestFixture]
    public class CourierDispatcherTests
    {
        ServiceProvider _provider;
        CourierEntity _courier;
        Guid _courierId;
        AddressEntity _address;
        OrderEntity _order;

        [OneTimeSetUp]
        public async Task Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            
            _provider = new ServiceCollection()
                .AddSingleton<ICourierDispatcher, CourierDispatcher>()
                .AddDbContext<OrdersDbContext>(x =>
                    x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")))
                .BuildServiceProvider();
            
            string[] cities = {
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
            
            string[] streets = {
                "123 E. 12th St.",
                "425 Broadway Ave.",
                "948 West St.",
                "99 California St.",
                "123 E. 14th Blvd.",
                "939 Crenshaw Blvd.",
                "294 E. Cotati Blvd."
            };
            
            string[] firstNames = {"Albert", "Christy", "Jose", "Stephen", "Michael", "Sarah", "Mia"};
            string[] lastNames = {"Jones", "Lacey", "Jordan", "Curry", "Wiseman", "Chavez", "Williams"};
            string[] zipCodes = {"93483", "9230", "83324", "93924", "82474", "69843", "73934"};

            var db = _provider.GetService<OrdersDbContext>();
            
            var regions = db.Regions.Select(x => x.RegionId).ToList();
            var address = db.Addresses
                .OrderBy(x => x.AddressId)
                .Select(x => x.AddressId)
                .ToList();
            var addressFaker = new Faker<AddressEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.AddressId, s => address.Last() + 1)
                .RuleFor(x => x.City, s => s.PickRandom(cities))
                .RuleFor(x => x.Street, s => s.PickRandom(streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(regions))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(zipCodes))
                .RuleFor(x => x.CreationTimestamp, DateTime.Now);

            _courierId = NewId.NextGuid();
            _address = addressFaker.Generate(1).FirstOrDefault();
            
            var courierFaker = new Faker<CourierEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CourierId, s => _courierId)
                .RuleFor(x => x.Status, s => (int)CourierStatus.Idle)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.FirstName, s => s.PickRandom(firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(lastNames))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.AddressId, s => _address.AddressId)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            _courier = courierFaker.Generate(1).FirstOrDefault();
            var orderFaker = new Faker<OrderEntity>()
                .StrictMode(true)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Courier)
                .Ignore(x => x.Restaurant)
                .Ignore(x => x.Address)
                .RuleFor(x => x.OrderId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 2))
                .RuleFor(x => x.CustomerId, s =>
                {
                    var customers = db.Customers.Select(c => c.CustomerId).ToList();
                    return s.PickRandom(customers);
                })
                .RuleFor(x => x.RestaurantId, s =>
                {
                    var restaurants = db.Restaurants.Select(r => r.RestaurantId).ToList();
                    return s.PickRandom(restaurants);
                })
                .RuleFor(x => x.AddressId, s =>
                {
                    var addresses = db.Addresses.Select(x => x.AddressId).ToList();
                    return s.PickRandom(addresses);
                })
                .RuleFor(x => x.CourierId, s => null)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            _order = orderFaker.Generate(1).FirstOrDefault();
            
            await db.Orders.AddAsync(_order);
            await db.Addresses.AddAsync(_address);
            await db.Couriers.AddAsync(_courier);

            await db.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            var db = _provider.GetService<OrdersDbContext>();

            var courier = await db.Couriers.FindAsync(_courierId);

            if (courier != null)
            {
                db.Couriers.Remove(courier);

                await db.SaveChangesAsync();
            }
        }

        [TearDown]
        public async Task TestTeardown()
        {
            var db = _provider.GetService<OrdersDbContext>();

            var courier = await db.Couriers.FindAsync(_courierId);

            if (courier != null)
            {
                courier.Status = (int)CourierStatus.Idle;

                db.Update(courier);

                await db.SaveChangesAsync();
            }
        }
        
        [Test]
        public async Task Verify_can_confirm_courier_dispatch()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            Result<Courier> result = await dispatcher.Confirm(_courierId);

            Assert.AreEqual((int)CourierStatus.Confirmed, result.Value.Status);
        }
        
        [Test]
        public async Task Verify_can_confirm_courier_order_pickup()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            Result<Order> result = await dispatcher.PickUpOrder(new OrderPickUpCriteria
            {
                OrderId = _order.OrderId,
                CourierId = _courierId
            });

            Assert.AreEqual((int)CourierStatus.PickedUpOrder, result.Value.Status);
            Assert.AreEqual(_courierId, result.Value.CourierId);
        }
        
        [Test]
        public async Task Verify_can_confirm_courier_order_delivered()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            Result<Order> result = await dispatcher.Deliver(new OrderDeliveryCriteria
            {
                OrderId = _order.OrderId,
                CourierId = _courierId
            });

            Assert.AreEqual((int)CourierStatus.DeliveredOrder, result.Value.Status);
        }

        [Test]
        public async Task Verify_can_dispatch_courier()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            var result = await dispatcher.Dispatch(new CourierDispatchCriteria
            {
                Street = "99 California St.",
                City = _address.City,
                RegionId = _address.RegionId,
                ZipCode = "69843"
            });
            
            Assert.AreEqual(_courierId, result.Value.CourierId);
            Assert.AreEqual((int)CourierStatus.Dispatched, result.Value.Status);
        }
    }
}