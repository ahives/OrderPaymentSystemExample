namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core.Events;

    public class OrderDeliveryConsumer :
        IConsumer<DeliverOrder>
    {
        public async Task Consume(ConsumeContext<DeliverOrder> context)
        {
            // await context.Publish<CourierEnRoute>(new
            // {
            //     context.Message.CourierId,
            //     context.Message.OrderId,
            //     context.Message.CustomerId,
            //     context.Message.RestaurantId
            // });
        }
    }
}