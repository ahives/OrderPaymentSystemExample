namespace Services.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class CookTests :
        DatabaseTestHarness
    {
        Guid _courierId;
        Guid _orderId;
        // Guid _orderItemId;
        Guid _menuItemId;

        public CookTests()
        {
            _provider = _services
                .AddSingleton<IPrepareOrder, PrepareOrder>()
                .BuildServiceProvider();
        }

        [OneTimeSetUp]
        protected async Task Setup()
        {
            var db = _provider.GetService<OrdersDbContext>();

            Guid regionId = NewId.NextGuid();
            Regions = GetRegionFaker(regionId).Generate(1);
            await db.AddRangeAsync(Regions);
            
            Guid temperatureId = NewId.NextGuid();
            
            Temperatures = GetTemperatureFaker(temperatureId).Generate(1);
            await db.AddRangeAsync(Temperatures);
            
            Guid addressId = NewId.NextGuid();
            
            Addresses = GetAddressFaker(addressId, regionId).Generate(1);
            await db.AddRangeAsync(Addresses);
            
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
            
            Couriers = GetCourierFaker(_courierId, true, addressId).Generate(1);
            await db.AddRangeAsync(Couriers);
            
            Guid shelfId = NewId.NextGuid();
            
            Shelves = GetShelfFaker(shelfId).Generate(1);
            await db.AddRangeAsync(Shelves);
            
            Orders = GetOrderFaker(customerId, restaurantId, addressId, _courierId).Generate(1);
            await db.AddRangeAsync(Orders);
            
            _orderId = db.Orders.Select(x => x.OrderId).ToList().Last();
            
            OrderItems = GetOrderItemFaker(_orderId, shelfId, _menuItemId).Generate(1);
            await db.AddRangeAsync(OrderItems);
            
            // _orderItemId = db.OrderItems.Select(x => x.OrderItemId).ToList().Last();

            await db.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            var db = _provider.GetService<OrdersDbContext>();

            db.Regions.RemoveRange(Regions);
            db.Addresses.RemoveRange(Addresses);
            db.Menus.RemoveRange(Menus);
            db.MenuItems.RemoveRange(MenuItems);
            db.Temperatures.RemoveRange(Temperatures);
            db.Customers.RemoveRange(Customers);
            db.Restaurants.RemoveRange(Restaurants);
            db.Orders.RemoveRange(Orders);
            db.OrderItems.RemoveRange(OrderItems);
            db.Shelves.RemoveRange(Shelves);
            db.Couriers.RemoveRange(Couriers);

            await db.SaveChangesAsync();
        }

        [TearDown]
        public async Task TestTeardown()
        {
            var db = _provider.GetService<OrdersDbContext>();

            // var courier = await db.Couriers.FindAsync(_courierId);
            //
            // if (courier != null)
            // {
            //     courier.Status = (int)CourierStatus.Idle;
            //
            //     db.Update(courier);
            //
            //     await db.SaveChangesAsync();
            // }
        }


        [Test]
        public async Task Test()
        {
            var cook = _provider.GetService<IPrepareOrder>();
            // var db = _provider.GetService<OrdersDbContext>();

            Guid orderItemId = NewId.NextGuid();
            var result = await cook.Prepare(new OrderPrepCriteria
            {
                OrderId = _orderId,
                OrderItemId = orderItemId,
                MenuItemId = _menuItemId,
                SpecialInstructions = "Cook light"
            });
            
            Assert.AreEqual(orderItemId, result.Value.OrderItemId);
            Assert.AreEqual((int)OrderItemStatus.Prepared, result.Value.Status);
        }
    }
}