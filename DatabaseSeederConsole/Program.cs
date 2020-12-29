namespace DatabaseSeederConsole
{
    using System.Threading.Tasks;
    using Data.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            var services = new ServiceCollection();

            services.AddSingleton<IOrdersDataGenerator, OrdersDataGenerator>();
            services.AddDbContext<OrdersDbContext>(x =>
                x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")));

            var provider = services.BuildServiceProvider();
            
            var generator = provider.GetService<IOrdersDataGenerator>();
            
            using (var context = provider.GetService<OrdersDbContext>())
            {
                await context.Couriers.AddRangeAsync(generator.Couriers);
                await context.Customers.AddRangeAsync(generator.Customers);
                await context.Menus.AddRangeAsync(generator.Menus);
                await context.MenuItems.AddRangeAsync(generator.MenuItems);
                await context.OrderItems.AddRangeAsync(generator.OrderItems);
                await context.Orders.AddRangeAsync(generator.Orders);
                await context.Regions.AddRangeAsync(generator.Regions);
                await context.Restaurants.AddRangeAsync(generator.Restaurants);
                await context.Shelves.AddRangeAsync(generator.Shelves);
                await context.Temperatures.AddRangeAsync(generator.Temperatures);

                await context.SaveChangesAsync();
            }
        }
    }
}