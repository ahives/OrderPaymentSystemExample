namespace CourierService.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Consumers;
    using Data.Core;
    using MassTransit;
    using MassTransit.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Service.Grpc.Core;
    using Services.Core;
    using Services.Core.Events;
    using Services.Core.Tests;

    [TestFixture]
    public class CourierConfirmationTests :
        DatabaseTestHarness
    {
        readonly ServiceProvider _provider;
        Guid _courierId;
        Guid _orderId;
        Guid _menuItemId;

        public CourierConfirmationTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            _provider = new ServiceCollection()
                .AddSingleton<ICourierDispatcher, CourierDispatcher>()
                .AddMassTransitInMemoryTestHarness(x =>
                {
                    x.AddConsumer<CourierDispatchConfirmationConsumer>();
                })
                .AddDbContext<OrdersDbContext>(x =>
                    x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")))
                .BuildServiceProvider();
        }

        [SetUp]
        public async Task TestSetup()
        {
            var db = _provider.GetService<OrdersDbContext>();

            Guid regionId = NewId.NextGuid();
            
            Regions = GetRegionFaker(regionId).Generate(1);
            await db.AddRangeAsync(Regions);
            
            Guid temperatureId = NewId.NextGuid();
            
            Temperatures = GetTemperatureFaker(temperatureId).Generate(1);
            await db.AddRangeAsync(Temperatures);
            
            // long addressId = db.Addresses.Select(x => x.AddressId).ToList().Last() + 1;
            //
            // Addresses = GetAddressFaker(addressId, regionId).Generate(1);
            // await db.AddRangeAsync(Addresses);
            
            Guid addressId = NewId.NextGuid();
            
            Restaurants = GetRestaurantFaker(addressId).Generate(1);
            await db.AddRangeAsync(Restaurants);

            Guid restaurantId = db.Restaurants.Select(x => x.RestaurantId).ToList().Last();
            
            Menus = GetMenuFaker(NewId.NextGuid(), restaurantId).Generate(1);
            await db.AddRangeAsync(Menus);
            
            Guid menuId = db.Menus.Select(x => x.MenuId).ToList().Last();
            
            MenuItems = GetMenuItemFaker(temperatureId, menuId).Generate(1);
            await db.AddRangeAsync(MenuItems);
            
            _menuItemId = db.MenuItems.Select(x => x.MenuItemId).ToList().Last();
            
            Customers = GetCustomers(addressId).Generate(1);
            await db.AddRangeAsync(Customers);
            
            Guid customerId = db.Customers.Select(x => x.CustomerId).ToList().Last();
            _courierId = NewId.NextGuid();
            
            Couriers = GetCourierFaker(_courierId, addressId).Generate(1);
            await db.AddRangeAsync(Couriers);
            
            // int shelfId = db.Shelves.Select(x => x.ShelfId).ToList().Last() + 1;
            //
            // Shelves = GetShelfFaker(shelfId).Generate(1);
            // await db.AddRangeAsync(Shelves);

            _orderId = NewId.NextGuid();
            
            Orders = GetOrderFaker(_orderId, customerId, restaurantId, addressId, _courierId).Generate(1);
            await db.AddRangeAsync(Orders);
            
            // _orderId = db.Orders.Select(x => x.OrderId).ToList().Last();
            
            OrderItems = GetOrderItemFaker(_orderId, null, _menuItemId).Generate(1);
            await db.AddRangeAsync(OrderItems);

            await db.SaveChangesAsync();
        }

        [TearDown]
        public async Task TestTeardown()
        {
            var db = _provider.GetService<OrdersDbContext>();

            db.Regions.RemoveRange(Regions);
            // db.Addresses.RemoveRange(Addresses);
            db.Menus.RemoveRange(Menus);
            db.MenuItems.RemoveRange(MenuItems);
            db.Temperatures.RemoveRange(Temperatures);
            db.Customers.RemoveRange(Customers);
            db.Restaurants.RemoveRange(Restaurants);
            db.Orders.RemoveRange(Orders);
            db.OrderItems.RemoveRange(OrderItems);
            // db.Shelves.RemoveRange(Shelves);
            db.Couriers.RemoveRange(Couriers);

            await db.SaveChangesAsync();
        }
        
        [Test]
        public async Task Test()
        {
            var harness = _provider.GetRequiredService<InMemoryTestHarness>();
            var consumer = harness.Consumer(() => _provider.GetRequiredService<CourierDispatchConfirmationConsumer>());
            
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<ConfirmCourierDispatch>(new
                {
                    OrderId = NewId.NextGuid(),
                    CourierId = NewId.NextGuid(),
                    // CourierId = Guid.Parse("11220000-4800-acde-2018-08d8ad59da7d"),
                    CustomerId = NewId.NextGuid(),
                    RestaurantId = NewId.NextGuid()
                });

                Assert.That(await harness.Consumed.Any<ConfirmCourierDispatch>());
                Assert.That(await consumer.Consumed.Any<ConfirmCourierDispatch>());
                // Assert.That(await harness.Published.Any<CourierConfirmed>());
                Assert.That(await harness.Published.Any<CourierDispatchDeclined>());
                Assert.That(await harness.Published.Any<Fault<ConfirmCourierDispatch>>(), Is.False);
            }
            finally
            {
                await harness.Stop();
                // await _provider.DisposeAsync();
            }
        }
        
        [Test]
        public async Task Test2()
        {
            var harness = _provider.GetRequiredService<InMemoryTestHarness>();
            var consumer = harness.Consumer(() => _provider.GetRequiredService<CourierDispatchConfirmationConsumer>());
            
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<ConfirmCourierDispatch>(new
                {
                    OrderId = _orderId,
                    CourierId = _courierId,
                    CustomerId = NewId.NextGuid(),
                    RestaurantId = NewId.NextGuid()
                });

                Assert.That(await harness.Consumed.Any<ConfirmCourierDispatch>());
                Assert.That(await consumer.Consumed.Any<ConfirmCourierDispatch>());
                Assert.That(await harness.Published.Any<CourierDispatchConfirmed>());
                // Assert.That(await harness.Published.Any<CourierDeclined>());
                Assert.That(await harness.Published.Any<Fault<ConfirmCourierDispatch>>(), Is.False);
            }
            finally
            {
                await harness.Stop();
                // await _provider.DisposeAsync();
            }
        }
    }
}