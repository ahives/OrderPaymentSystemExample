namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Services.Core.Events;

    public class CourierEnRouteConsumer :
        IConsumer<CourierEnRouteToRestaurant>
    {
        public async Task Consume(ConsumeContext<CourierEnRouteToRestaurant> context)
        {
            Log.Information($"Consumer - {nameof(CourierEnRouteConsumer)}");
            
            await context.Publish<OrderDelivered>(new
            {
                context.Message.CourierId,
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.RestaurantId
            });
        }
    }
}