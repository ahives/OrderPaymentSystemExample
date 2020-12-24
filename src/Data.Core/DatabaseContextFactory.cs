namespace Data.Core
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DatabaseContextFactory :
        IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(configuration.GetConnectionString("OrdersConnection"));

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}