namespace Data.Core
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class OrdersDbContextFactory :
        IDesignTimeDbContextFactory<OrdersDbContext>
    {
        public OrdersDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseNpgsql(configuration.GetConnectionString("OrdersConnection"), options => options.EnableRetryOnFailure());

            return new OrdersDbContext(optionsBuilder.Options);
        }
    }
}