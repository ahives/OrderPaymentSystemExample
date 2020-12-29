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
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<ShelfEntity> Shelves { get; set; }
        public DbSet<StorageTemperatureEntity> Temperatures { get; set; }
        public DbSet<RegionEntity> Regions { get; set; }

        public OrdersDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}