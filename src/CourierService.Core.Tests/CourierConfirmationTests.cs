namespace CourierService.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Consumers;
    using Data.Core;
    using MassTransit;
    using MassTransit.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Services.Core;
    using Services.Core.Events;

    [TestFixture]
    public class CourierConfirmationTests
    {
        ServiceProvider _provider;

        [OneTimeSetUp]
        public void Setup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            _provider = new ServiceCollection()
                .AddSingleton<ICourierDispatcher, CourierDispatcher>()
                .AddMassTransitInMemoryTestHarness(x =>
                {
                    x.AddConsumer<CourierConfirmationConsumer>();
                })
                .AddDbContext<OrdersDbContext>(x =>
                    x.UseNpgsql(configuration.GetConnectionString("OrdersConnection")))
                .BuildServiceProvider();
        }
        
        [Test]
        public async Task Test()
        {
            var harness = _provider.GetRequiredService<InMemoryTestHarness>();
            var consumer = harness.Consumer(() => _provider.GetRequiredService<CourierConfirmationConsumer>());
            
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<ConfirmCourier>(new
                {
                    OrderId = NewId.NextGuid(),
                    CourierId = Guid.Parse("11220000-4800-acde-2018-08d8ad59da7d"),
                    CustomerId = NewId.NextGuid(),
                    RestaurantId = NewId.NextGuid()
                });

                Assert.That(await harness.Consumed.Any<ConfirmCourier>());
                Assert.That(await consumer.Consumed.Any<ConfirmCourier>());
                Assert.That(await harness.Published.Any<CourierConfirmed>());
                Assert.That(await harness.Published.Any<Fault<CourierConfirmed>>(), Is.False);
            }
            finally
            {
                await harness.Stop();
                await _provider.DisposeAsync();
            }
        }
    }
}