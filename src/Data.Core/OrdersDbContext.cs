namespace Data.Core
{
    using Microsoft.EntityFrameworkCore;
    using Model;

    public class OrdersDbContext :
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
        public DbSet<StorageTemperature> Temperatures { get; set; }

        public OrdersDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IOrdersDataGenerator generator = new OrdersDataGenerator();
            
            modelBuilder.Entity<Region>().HasData(generator.Regions);
            modelBuilder.Entity<StorageTemperature>().HasData(generator.Temperatures);
            modelBuilder.Entity<Restaurant>().HasData(generator.Restaurants);
            modelBuilder.Entity<Menu>().HasData(generator.Menus);
            modelBuilder.Entity<MenuItem>().HasData(generator.MenuItems);
            modelBuilder.Entity<Customer>().HasData(generator.Customers);
            modelBuilder.Entity<Order>().HasData(generator.Orders);
            modelBuilder.Entity<OrderItem>().HasData(generator.OrderItems);
            modelBuilder.Entity<Courier>().HasData(generator.Couriers);
            modelBuilder.Entity<Shelf>().HasData(generator.Shelves);
        }
    }
}