namespace Services.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class CourierFinderTests
    {
        [Test]
        public async Task Test()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
            var services = new ServiceCollection();

            services.AddSingleton<ICourierFinder, CourierFinder>();
            services.AddDbContext<OrdersDbContext>(x =>
                x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")));

            var provider = services.BuildServiceProvider();

            var finder = provider.GetService<ICourierFinder>();

            var courier = await finder.Find(new CourierFinderRequest
            {
                Street = "99 California St.",
                City = "Philadelphia",
                RegionId = 2,
                ZipCode = "69843"
            });
            
            Assert.AreEqual(Guid.Parse("11220000-4800-acde-be22-08d8ac16f995"), courier.Value.CourierId);
        }
    }
}