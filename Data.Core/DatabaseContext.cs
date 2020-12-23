namespace DatabaseDeploy
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
            
            var regions = GetRegions();
            var storageTemperatures = GetStorageTemperatures();
            var menuItems = GetMenuItems(menuIds);
            var menus = GetMenus(restaurantIds, menuIds);
            var restaurants = GetRestaurants(restaurantIds);
            
            modelBuilder.Entity<Region>().HasData(regions);
            modelBuilder.Entity<StorageTemperature>().HasData(storageTemperatures);
            modelBuilder.Entity<Restaurant>().HasData(restaurants);
            modelBuilder.Entity<Menu>().HasData(menus);
            modelBuilder.Entity<MenuItem>().HasData(menuItems);
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

        IEnumerable<MenuItem> GetMenuItems(List<Guid> menuIds)
        {
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Bacon, Egg, and Cheese Burrito",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Blueberry Pancakes",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Hot Chocolate",
                MenuId = menuIds[0],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Milk",
                MenuId = menuIds[0],
                StorageTemperatureId = 2,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Orange Juice",
                MenuId = menuIds[0],
                StorageTemperatureId = 2,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Steak",
                MenuId = menuIds[1],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Rice and Gravy",
                MenuId = menuIds[1],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Cheese Pizza",
                MenuId = menuIds[2],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Pepperoni Pizza",
                MenuId = menuIds[2],
                StorageTemperatureId = 1,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Lemonade Smoothie",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Pineapple Smoothie",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
            yield return new MenuItem
            {
                MenuItemId = NewId.NextGuid(),
                Name = "Chocolate Ice Cream",
                MenuId = menuIds[3],
                StorageTemperatureId = 3,
                CreationTimestamp = DateTime.Now
            };
        }
    }
}