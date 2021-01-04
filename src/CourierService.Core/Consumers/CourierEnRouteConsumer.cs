namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core.Events;

    public class CourierEnRouteConsumer :
        IConsumer<CourierEnRouteToRestaurant>
    {
        public async Task Consume(ConsumeContext<CourierEnRouteToRestaurant> context)
        {
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