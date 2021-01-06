namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Services.Core.Events;

    public class CourierEnRouteToRestaurantConsumer :
        IConsumer<CourierEnRouteToRestaurant>
    {
        public async Task Consume(ConsumeContext<CourierEnRouteToRestaurant> context)
        {
            Log.Information($"Consumer - {nameof(CourierEnRouteToRestaurantConsumer)}");
        }
    }
}