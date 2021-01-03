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
        public DbSet<TemperatureEntity> Temperatures { get; set; }
        public DbSet<RegionEntity> Regions { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }
        public DbSet<IngredientEntity> Ingredients { get; set; }
        public DbSet<InventoryItemEntity> InventoryItems { get; set; }
        public DbSet<MenuItemIngredientEntity> MenuItemIngredients { get; set; }

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
        {
        }
    }
}