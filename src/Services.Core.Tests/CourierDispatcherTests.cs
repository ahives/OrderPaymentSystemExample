namespace Services.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Grpc.Net.Client;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Model;
    using NUnit.Framework;
    using ProtoBuf.Grpc.Client;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;

    [TestFixture]
    public class CourierDispatcherTests :
        DatabaseTestHarness
    {
        Guid _courierId;
        Guid _orderId;
        Guid _menuItemId;

        public CourierDispatcherTests()
        {
            _services.AddSingleton<IGrpcClient<ICourierDispatcher>, CourierDispatcherClient>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            _services.AddScoped<IConfiguration>(_ => configuration);
            
            _provider = _services
                // .AddSingleton<ICourierDispatcher, CourierDispatcher>()
                .BuildServiceProvider();
        }

        // [OneTimeSetUp]
        // public async Task Setup()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //
        //     Guid regionId = NewId.NextGuid();
        //     Regions = GetRegionFaker(regionId).Generate(1);
        //     await db.AddRangeAsync(Regions);
        //     
        //     Guid temperatureId = NewId.NextGuid();
        //     
        //     Temperatures = GetTemperatureFaker(temperatureId).Generate(1);
        //     await db.AddRangeAsync(Temperatures);
        //     
        //     Guid addressId = NewId.NextGuid();
        //     
        //     // Addresses = GetAddressFaker(addressId, regionId).Generate(1);
        //     // await db.AddRangeAsync(Addresses);
        //     
        //     Restaurants = GetRestaurantFaker(addressId).Generate(1);
        //     await db.AddRangeAsync(Restaurants);
        //
        //     Guid restaurantId = db.Restaurants.Select(x => x.RestaurantId).ToList().Last();
        //     
        //     Menus = GetMenuFaker(NewId.NextGuid(), restaurantId).Generate(1);
        //     await db.AddRangeAsync(Menus);
        //     
        //     Guid menuId = db.Menus.Select(x => x.MenuId).ToList().Last();
        //     
        //     MenuItems = GetMenuItemFaker(temperatureId, menuId).Generate(1);
        //     await db.AddRangeAsync(MenuItems);
        //     
        //     _menuItemId = db.MenuItems.Select(x => x.MenuItemId).ToList().Last();
        //     
        //     Customers = GetCustomers(addressId).Generate(1);
        //     await db.AddRangeAsync(Customers);
        //     
        //     Guid customerId = db.Customers.Select(x => x.CustomerId).ToList().Last();
        //     _courierId = NewId.NextGuid();
        //     
        //     Couriers = GetCourierFaker(_courierId, true, addressId).Generate(1);
        //     await db.AddRangeAsync(Couriers);
        //     
        //     Guid shelfId = NewId.NextGuid();
        //     
        //     // Shelves = GetShelfFaker(shelfId).Generate(1);
        //     // await db.AddRangeAsync(Shelves);
        //     
        //     Orders = GetOrderFaker(customerId, restaurantId, addressId, _courierId).Generate(1);
        //     await db.AddRangeAsync(Orders);
        //     
        //     _orderId = db.Orders.Select(x => x.OrderId).ToList().Last();
        //     
        //     OrderItems = GetOrderItemFaker(_orderId, shelfId, _menuItemId).Generate(1);
        //     await db.AddRangeAsync(OrderItems);
        //
        //     await db.SaveChangesAsync();
        // }
        //
        // [OneTimeTearDown]
        // public async Task Teardown()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //
        //     db.Regions.RemoveRange(Regions);
        //     // db.Addresses.RemoveRange(Addresses);
        //     db.Menus.RemoveRange(Menus);
        //     db.MenuItems.RemoveRange(MenuItems);
        //     db.Temperatures.RemoveRange(Temperatures);
        //     db.Customers.RemoveRange(Customers);
        //     db.Restaurants.RemoveRange(Restaurants);
        //     db.Orders.RemoveRange(Orders);
        //     db.OrderItems.RemoveRange(OrderItems);
        //     // db.Shelves.RemoveRange(Shelves);
        //     db.Couriers.RemoveRange(Couriers);
        //
        //     await db.SaveChangesAsync();
        // }
        //
        // [TearDown]
        // public async Task TestTeardown()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //
        //     var courier = await db.Couriers.FindAsync(_courierId);
        //
        //     if (courier != null)
        //     {
        //         courier.Status = (int)CourierStatus.Idle;
        //
        //         db.Update(courier);
        //
        //         await db.SaveChangesAsync();
        //     }
        // }

        [Test]
        public async Task Test()
        {
            var client = _provider.GetService<IGrpcClient<ICourierDispatcher>>();
            
            var result = await client.Client.ChangeStatus(new ()
            {
                // CourierId = Guid.Parse("11220000-4800-acde-d916-08d8b0f69e93"),
                // CourierId = Guid.Parse("11220000-4800-acde-50fa-08d8b42ba8ff"),
                CourierId = Guid.Parse("11220000-4800-acde-50fa-08d8b42ba8ff"),
                // OrderId = Guid.Parse("11220000-4800-acde-3288-08d8b42ba903"),
                // RestaurantId = Guid.Parse(""),
                Status = CourierStatus.Dispatched
            });
            
            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public async Task Test2()
        {
            var client = _provider.GetService<IGrpcClient<ICourierDispatcher>>();
            
            var result = await client.Client.Identify(new ()
            {
                CustomerId = Guid.Parse("11220000-4800-acde-1760-08d8b42ba8e2"),
            });
            
            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public async Task Verify_can_confirm_courier_dispatch()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            var db = _provider.GetService<OrdersDbContext>();

            var target = await db.Couriers.FirstOrDefaultAsync();
            
            // Result<Courier> result = await dispatcher.Confirm(target.CourierId);
            Result<Courier> result = await dispatcher.ChangeStatus(new ()
            {
                CourierId = _courierId,
                Status = CourierStatus.DispatchConfirmed
            });

            Assert.AreEqual((int)CourierStatus.DispatchConfirmed, result.Value.Status);
        }
        
        [Test]
        public async Task Verify_can_confirm_courier_order_pickup()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            // var db = _provider.GetService<OrdersDbContext>();
            
            var result = await dispatcher.PickUpOrder(new CourierDispatchContext
            {
                OrderId = _orderId,
                CourierId = _courierId
            });

            Assert.AreEqual((int)CourierStatus.PickedUpOrder, result.Value.Status);
            Assert.AreEqual(_courierId, result.Value.CourierId);
        }
        
        [Test]
        public async Task Verify_can_confirm_courier_order_delivered()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            var result = await dispatcher.Deliver(new CourierDispatchContext
            {
                OrderId = _orderId,
                CourierId = _courierId
            });

            Assert.AreEqual((int)CourierStatus.DeliveredOrder, result.Value.Status);
        }

        [Test]
        public async Task Verify_can_dispatch_courier()
        {
            var dispatcher = _provider.GetService<ICourierDispatcher>();

            var result = await dispatcher.ChangeStatus(new ()
            {
                // CourierId = ,
                Status = CourierStatus.Dispatched
            });
            
            Assert.AreEqual(_courierId, result.Value.CourierId);
            Assert.AreEqual((int)CourierStatus.Dispatched, result.Value.Status);
        }
    }
}