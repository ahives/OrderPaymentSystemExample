namespace Data.Core
{
    using System;
    using System.Collections.Generic;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class DatabaseContext :
        DbContext
    {
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Shelf> Shelves { get; set; }

        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=Orders;Username=admin;Password=");

            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var restaurantIds = new List<Guid>
            {
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid()
            };
            var menuIds = new List<Guid>
            {
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid()
            };
            var menuItemIds = new List<Guid>
            {
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid()
            };
            var orderIds = new List<Guid>
            {
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid()
            };
            var customerIds = new List<Guid>
            {
                NewId.NextGuid(),
                NewId.NextGuid(),
                NewId.NextGuid()
            };
            
            var regions = GetRegions();
            var storageTemperatures = GetStorageTemperatures();
            var menuItems = GetMenuItems(menuItemIds, menuIds);
            var menus = GetMenus(restaurantIds, menuIds);
            var restaurants = GetRestaurants(restaurantIds);
            var orders = GetOrders(orderIds, customerIds);
            var orderItems = GetOrderItems(orderIds, menuItemIds);
            var customers = GetCustomers(customerIds);
            var shelves = GetShelves();
            
            modelBuilder.Entity<Region>().HasData(regions);
            modelBuilder.Entity<StorageTemperature>().HasData(storageTemperatures);
            modelBuilder.Entity<Restaurant>().HasData(restaurants);
            modelBuilder.Entity<Menu>().HasData(menus);
            modelBuilder.Entity<MenuItem>().HasData(menuItems);
            modelBuilder.Entity<Customer>().HasData(customers);
            modelBuilder.Entity<Order>().HasData(orders);
            modelBuilder.Entity<OrderItem>().HasData(orderItems);
            modelBuilder.Entity<Shelf>().HasData(shelves);
        }

        IEnumerable<Shelf> GetShelves()
        {
            yield return new Shelf
            {
                ShelfId = 1,
                Name = "Hot",
                StorageTemperatureId = 1,
                Capacity = 10,
                CreationTimestamp = DateTime.Now
            };
            yield return new Shelf
            {
                ShelfId = 2,
                Name = "Cold",
                StorageTemperatureId = 2,
                Capacity = 10,
                CreationTimestamp = DateTime.Now
            };
            yield return new Shelf
            {
                ShelfId = 3,
                Name = "Frozen",
                StorageTemperatureId = 3,
                Capacity = 10,
                CreationTimestamp = DateTime.Now
            };
            yield return new Shelf
            {
                ShelfId = 4,
                Name = "Overflow",
                StorageTemperatureId = 4,
                Capacity = 15,
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<Customer> GetCustomers(List<Guid> customerIds)
        {
            yield return new Customer
            {
                CustomerId = customerIds[0],
                FirstName = "Iron",
                LastName = "Man",
                Street = "123 E. 12th Street",
                City = "Oakland",
                RegionId = 1,
                ZipCode = "94123",
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<Order> GetOrders(List<Guid> orderIds, List<Guid> customerIds)
        {
            yield return new Order
            {
                OrderId = orderIds[0],
                CourierId = null,
                CustomerId = customerIds[0],
                Street = "93rd Olive Street",
                City = "Oakland",
                RegionId = 1,
                ZipCode = "94543",
                Status = 1,
                StatusTimestamp = DateTime.Now
            };
            yield return new Order
            {
                OrderId = orderIds[1],
                CourierId = null,
                CustomerId = customerIds[0],
                Street = "93rd Olive Street",
                City = "Oakland",
                RegionId = 1,
                ZipCode = "94543",
                Status = 1,
                StatusTimestamp = DateTime.Now
            };
            yield return new Order
            {
                OrderId = orderIds[2],
                CourierId = null,
                CustomerId = customerIds[0],
                Street = "93rd Olive Street",
                City = "Oakland",
                RegionId = 1,
                ZipCode = "94543",
                Status = 1,
                StatusTimestamp = DateTime.Now
            };
        }

        IEnumerable<OrderItem> GetOrderItems(List<Guid> orderIds, List<Guid> menuItemIds)
        {
            yield return new OrderItem
            {
                OrderItemId = NewId.NextGuid(),
                OrderId = orderIds[0],
                MenuItemId = menuItemIds[0],
                IsExpired = true,
                ShelfId = 1,
                Status = OrderItemStatus.New,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                CreationTimestamp = DateTime.Now
            };
            yield return new OrderItem
            {
                OrderItemId = NewId.NextGuid(),
                OrderId = orderIds[0],
                MenuItemId = menuItemIds[1],
                IsExpired = false,
                ShelfId = 1,
                Status = OrderItemStatus.Prepared,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                CreationTimestamp = DateTime.Now
            };
            yield return new OrderItem
            {
                OrderItemId = NewId.NextGuid(),
                OrderId = orderIds[1],
                MenuItemId = menuItemIds[2],
                IsExpired = true,
                ShelfId = 1,
                Status = OrderItemStatus.Prepared,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<StorageTemperature> GetStorageTemperatures()
        {
            yield return new StorageTemperature
            {
                StorageTemperatureId = 1,
                Name = "Hot",
                CreationTimestamp = DateTime.Now
            };
            yield return new StorageTemperature
            {
                StorageTemperatureId = 2,
                Name = "Cold",
                CreationTimestamp = DateTime.Now
            };
            yield return new StorageTemperature
            {
                StorageTemperatureId = 3,
                Name = "Frozen",
                CreationTimestamp = DateTime.Now
            };
            yield return new StorageTemperature
            {
                StorageTemperatureId = 4,
                Name = "Overflow",
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<Region> GetRegions()
        {
            yield return new Region
            {
                RegionId = 1,
                Name = "California",
                CreationTimestamp = DateTime.Now
            };
            yield return new Region
            {
                RegionId = 2,
                Name = "New York",
                CreationTimestamp = DateTime.Now
            };
            yield return new Region
            {
                RegionId = 3,
                Name = "Georgia",
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<Restaurant> GetRestaurants(List<Guid> restaurantIds)
        {
            yield return new Restaurant
            {
                RestaurantId = restaurantIds[0],
                Name = "Breakfast Stuff",
                Street = "1234 Broadway Blvd.",
                City = "Oakland",
                RegionId = 1,
                ZipCode = "94123",
                CreationTimestamp = DateTime.Now
            };
            yield return new Restaurant
            {
                RestaurantId = restaurantIds[1],
                Name = "All Day Stuff",
                Street = "1235 17th Street",
                City = "New York",
                RegionId = 2,
                ZipCode = "94132",
                CreationTimestamp = DateTime.Now
            };
            yield return new Restaurant
            {
                RestaurantId = restaurantIds[2],
                Name = "Desert Stuff",
                Street = "8973 E. 12th Street",
                City = "Atlanta",
                RegionId = 3,
                ZipCode = "94132",
                CreationTimestamp = DateTime.Now
            };
        }

        IEnumerable<Menu> GetMenus(List<Guid> restaurantIds, List<Guid> menuIds)
        {
            yield return new Menu
            {
                MenuId = menuIds[0],
                Name = "Breakfast",
                IsActive = true,
                RestaurantId = restaurantIds[0],
                CreationTimestamp = DateTime.Now,
            };
            yield return new Menu
            {
                MenuId = menuIds[1],
                Name = "Dinner",
                IsActive = true,
                RestaurantId = restaurantIds[0],
                CreationTimestamp = DateTime.Now,
            };
            yield return new Menu
            {
                MenuId = menuIds[2],
                Name = "Lunch & Dinner",
                IsActive = true,
                RestaurantId = restaurantIds[1],
                CreationTimestamp = DateTime.Now,
            };
            yield return new Menu
            {
                MenuId = menuIds[3],
                Name = "All Day",
                IsActive = true,
                RestaurantId = restaurantIds[2],
                CreationTimestamp = DateTime.Now,
            };
        }

        IEnumerable<MenuItem> GetMenuItems(List<Guid> menuItemIds, List<Guid> menuIds)
        {
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[0],
                Name = "Bacon, Egg, and Cheese Burrito",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[1],
                Name = "Blueberry Pancakes",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[2],
                Name = "Hot Chocolate",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[3],
                Name = "Milk",
                MenuId = menuIds[0],
                StorageTemperatureId = 2,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[4],
                Name = "Orange Juice",
                MenuId = menuIds[0],
                StorageTemperatureId = 2,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[5],
                Name = "Steak",
                MenuId = menuIds[1],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[6],
                Name = "Rice and Gravy",
                MenuId = menuIds[1],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[7],
                Name = "Cheese Pizza",
                MenuId = menuIds[2],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[8],
                Name = "Pepperoni Pizza",
                MenuId = menuIds[2],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[9],
                Name = "Lemonade Smoothie",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[10],
                Name = "Pineapple Smoothie",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = menuItemIds[11],
                Name = "Chocolate Ice Cream",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
        }
    }
}