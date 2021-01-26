namespace CourierService.Core.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Consumers;
    using Data.Core;
    using Data.Core.Model;
    using Grpc.Net.Client;
    using MassTransit;
    using MassTransit.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Moq.Protected;
    using NUnit.Framework;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;
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
        Guid _customerId;
        EntityEntry<CourierEntity> _courier;
        EntityEntry<RegionEntity> _customerRegion;
        EntityEntry<RegionEntity> _restaurantRegion;
        EntityEntry<AddressEntity> _customerAddress;
        EntityEntry<RestaurantEntity> _restaurant;
        EntityEntry<OrderEntity> _order;
        EntityEntry<CustomerEntity> _customer;
        EntityEntry<AddressEntity> _restaurantAddress;
        EntityEntry<RegionEntity> _courierRegion;
        EntityEntry<AddressEntity> _courierAddress;

        public CourierConfirmationTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            _provider = new ServiceCollection()
                .AddSingleton<IGrpcClient<ICourierDispatcher>, CourierDispatcherClient>()
                .AddScoped(_ => configuration)
                .AddMassTransitInMemoryTestHarness(x =>
                {
                    x.AddConsumer<DispatchConfirmationConsumer>();
                })
                .AddDbContext<OrdersDbContext>(x =>
                    x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")))
                .BuildServiceProvider();
        }

        // [OneTimeSetUp]
        // public async Task Setup()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //
        //     Guid restaurantRegionId = NewId.NextGuid();
        //
        //     _restaurantRegion = await db.Regions.AddAsync(new RegionEntity()
        //     {
        //         RegionId = restaurantRegionId,
        //         Name = "California",
        //         CreationTimestamp = DateTime.Now
        //     });
        //
        //     Guid customerRegionId = NewId.NextGuid();
        //
        //     _customerRegion = await db.Regions.AddAsync(new RegionEntity()
        //     {
        //         RegionId = customerRegionId,
        //         Name = "California",
        //         CreationTimestamp = DateTime.Now
        //     });
        //
        //     Guid courierRegionId = NewId.NextGuid();
        //
        //     _courierRegion = await db.Regions.AddAsync(new RegionEntity()
        //     {
        //         RegionId = courierRegionId,
        //         Name = "California",
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     Guid restaurantAddressId = NewId.NextGuid();
        //
        //     _restaurantAddress = await db.Addresses.AddAsync(new AddressEntity()
        //     {
        //         AddressId = restaurantAddressId,
        //         Street = "127 California St.",
        //         City = "Oakland",
        //         RegionId = restaurantRegionId,
        //         ZipCode = "94123",
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     Guid customerAddressId = NewId.NextGuid();
        //
        //     _customerAddress = await db.Addresses.AddAsync(new AddressEntity()
        //     {
        //         AddressId = customerAddressId,
        //         Street = "1234 Holly St.",
        //         City = "Oakland",
        //         RegionId = customerRegionId,
        //         ZipCode = "94103",
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     Guid courierAddressId = NewId.NextGuid();
        //
        //     _courierAddress = await db.Addresses.AddAsync(new AddressEntity()
        //     {
        //         AddressId = courierAddressId,
        //         Street = "863 Walnut St.",
        //         City = "Oakland",
        //         RegionId = courierRegionId,
        //         ZipCode = "94188",
        //         CreationTimestamp = DateTime.Now
        //     });
        //
        //     Guid courierId = NewId.NextGuid();
        //
        //     _courier = await db.Couriers.AddAsync(new CourierEntity()
        //     {
        //         CourierId = courierId,
        //         FirstName = "Lars",
        //         LastName = "Pittman",
        //         IsActive = true,
        //         Status = (int) CourierStatus.Idle,
        //         StatusTimestamp = DateTime.Now,
        //         AddressId = courierAddressId,
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     Guid restaurantId = NewId.NextGuid();
        //
        //     _restaurant = await db.Restaurants.AddAsync(new RestaurantEntity()
        //     {
        //         RestaurantId = restaurantId,
        //         AddressId = restaurantAddressId,
        //         IsActive = true,
        //         IsOpen = true,
        //         Name = "Big Al's",
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     Guid customerId = NewId.NextGuid();
        //
        //     _customer = await db.Customers.AddAsync(new CustomerEntity()
        //     {
        //         CustomerId = customerId,
        //         FirstName = "Joesph",
        //         LastName = "Somebody",
        //         AddressId = customerAddressId,
        //         CreationTimestamp = DateTime.Now
        //     });
        //     
        //     _order = await db.Orders.AddAsync(new OrderEntity()
        //     {
        //         CourierId = courierId,
        //         CustomerId = customerId,
        //         CustomerPickup = false,
        //         Status = (int)OrderStatus.Receipt,
        //         StatusTimestamp = DateTime.Now,
        //         RestaurantId = restaurantId,
        //         AddressId = customerAddressId,
        //         CreationTimestamp = DateTime.Now
        //     });
        //
        //     await db.SaveChangesAsync();
        // }
        //
        // [OneTimeTearDown]
        // public async Task Teardown()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //
        //     db.Couriers.Remove(_courier.Entity);
        //     db.Orders.Remove(_order.Entity);
        //     db.Customers.Remove(_customer.Entity);
        //     db.Restaurants.Remove(_restaurant.Entity);
        //     db.Addresses.Remove(_restaurantAddress.Entity);
        //     db.Addresses.Remove(_customerAddress.Entity);
        //     db.Addresses.Remove(_courierAddress.Entity);
        //     db.Regions.Remove(_restaurantRegion.Entity);
        //     db.Regions.Remove(_customerRegion.Entity);
        //     db.Regions.Remove(_courierRegion.Entity);
        //
        //     await db.SaveChangesAsync();
        // }
        
        // [SetUp]
        // public async Task TestSetup()
        // {
        //     var db = _provider.GetService<OrdersDbContext>();
        //     
        //     Guid regionId = NewId.NextGuid();
        //     
        //     Regions = GetRegionFaker(regionId).Generate(1);
        //     await db.AddRangeAsync(Regions);
        //     
        //     Guid temperatureId = NewId.NextGuid();
        //     
        //     Temperatures = GetTemperatureFaker(temperatureId).Generate(1);
        //     await db.AddRangeAsync(Temperatures);
        //     
        //     // long addressId = db.Addresses.Select(x => x.AddressId).ToList().Last() + 1;
        //     //
        //     // Addresses = GetAddressFaker(addressId, regionId).Generate(1);
        //     // await db.AddRangeAsync(Addresses);
        //     
        //     Guid addressId = NewId.NextGuid();
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
        //     Couriers = GetCourierFaker(_courierId, addressId).Generate(1);
        //     await db.AddRangeAsync(Couriers);
        //     
        //     // int shelfId = db.Shelves.Select(x => x.ShelfId).ToList().Last() + 1;
        //     //
        //     // Shelves = GetShelfFaker(shelfId).Generate(1);
        //     // await db.AddRangeAsync(Shelves);
        //
        //     _orderId = NewId.NextGuid();
        //     
        //     Orders = GetOrderFaker(_orderId, customerId, restaurantId, addressId, _courierId).Generate(1);
        //     await db.AddRangeAsync(Orders);
        //     
        //     // _orderId = db.Orders.Select(x => x.OrderId).ToList().Last();
        //     
        //     OrderItems = GetOrderItemFaker(_orderId, null, _menuItemId).Generate(1);
        //     await db.AddRangeAsync(OrderItems);
        //
        //     await db.SaveChangesAsync();
        // }
        //
        // [TearDown]
        // public async Task TestTeardown()
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
        
        [Test]
        public async Task Test()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var provider = new ServiceCollection()
                .AddSingleton<IGrpcClient<ICourierDispatcher>, FakeCourierDispatcherClient>()
                .AddScoped(_ => configuration)
                .AddMassTransitInMemoryTestHarness(x =>
                {
                    x.AddConsumer<DispatchConfirmationConsumer>();
                })
                .BuildServiceProvider();
            
            var harness = provider.GetRequiredService<InMemoryTestHarness>();
            var consumer = harness.Consumer(() => provider.GetRequiredService<DispatchConfirmationConsumer>());
            
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<ConfirmCourierDispatch>(new
                {
                    _order.Entity.OrderId,
                    _courier.Entity.CourierId,
                    _customer.Entity.CustomerId,
                    _restaurant.Entity.RestaurantId
                });

                Assert.That(await harness.Consumed.Any<ConfirmCourierDispatch>());
                Assert.That(await consumer.Consumed.Any<ConfirmCourierDispatch>());
                Assert.That(await harness.Published.Any<CourierDispatchConfirmed>(), Is.True);
                Assert.That(await harness.Published.Any<CourierDispatchDeclined>(), Is.False);
                Assert.That(await harness.Published.Any<Fault<ConfirmCourierDispatch>>(), Is.False);
            }
            finally
            {
                await harness.Stop();
            }
        }
        
        [Test]
        public async Task Test2()
        {
            var harness = _provider.GetRequiredService<InMemoryTestHarness>();
            var consumer = harness.Consumer(() => _provider.GetRequiredService<DispatchConfirmationConsumer>());
            
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

    public class FakeCourierDispatcherClient :
        IGrpcClient<ICourierDispatcher>
    {
        public ICourierDispatcher Client => new FakeCourierDispatcher();
        public GrpcChannel Channel { get; }
    }

    public class FakeCourierDispatcher : ICourierDispatcher
    {
        public async Task<Result<Courier>> Identify(CourierIdentificationContext context) => throw new NotImplementedException();

        public async Task<Result<Courier>> Decline(CourierDispatchContext context) => throw new NotImplementedException();

        public async Task<Result<Courier>> ChangeStatus(CourierStatusChangeContext context) =>
            new Result<Courier> {IsSuccessful = true};

        public async Task<Result<Order>> PickUpOrder(CourierDispatchContext context) => throw new NotImplementedException();

        public async Task<Result<Order>> Deliver(CourierDispatchContext context) => throw new NotImplementedException();
    }
}