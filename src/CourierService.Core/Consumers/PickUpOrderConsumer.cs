namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core.Events;

    public class PickUpOrderConsumer :
        IConsumer<PickUpOrder>
    {
        public async Task Consume(ConsumeContext<PickUpOrder> context)
        {
            await context.Publish<OrderPickedUp>(new
            {
                context.Message.CourierId,
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.RestaurantId
            });
        }
    }
}