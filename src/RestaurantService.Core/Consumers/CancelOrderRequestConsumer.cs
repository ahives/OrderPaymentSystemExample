namespace RestaurantService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core.Events;

    public class CancelOrderRequestConsumer :
        IConsumer<CancelOrderRequest>
    {
        public async Task Consume(ConsumeContext<CancelOrderRequest> context)
        {
            await context.Publish<OrderCanceled>(new
            {
                context.Message.OrderId,
                context.Message.CourierId,
                context.Message.CustomerId,
                context.Message.RestaurantId
            });
        }
    }
}