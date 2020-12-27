namespace Data.Core
{
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class OrdersDbContext :
        DbContext
    {
        public DbSet<MenuEntity> Menus { get; set; }
        public DbSet<MenuItemEntity> MenuItems { get; set; }
        public DbSet<RestaurantEntity> Restaurants { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<CourierEntity> Couriers { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShelfEntity> Shelves { get; set; }
        public DbSet<StorageTemperatureEntity> Temperatures { get; set; }

        public OrdersDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IOrdersDataGenerator generator = new OrdersDataGenerator();
            
            modelBuilder.Entity<RegionEntity>().HasData(generator.Regions);
            modelBuilder.Entity<StorageTemperatureEntity>().HasData(generator.Temperatures);
            modelBuilder.Entity<RestaurantEntity>().HasData(generator.Restaurants);
            modelBuilder.Entity<MenuEntity>().HasData(generator.Menus);
            modelBuilder.Entity<MenuItemEntity>().HasData(generator.MenuItems);
            modelBuilder.Entity<CustomerEntity>().HasData(generator.Customers);
            modelBuilder.Entity<OrderEntity>().HasData(generator.Orders);
            modelBuilder.Entity<OrderItem>().HasData(generator.OrderItems);
            modelBuilder.Entity<CourierEntity>().HasData(generator.Couriers);
            modelBuilder.Entity<ShelfEntity>().HasData(generator.Shelves);
        }
    }
}