namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core.Events;

    public class CourierConfirmationConsumer :
        IConsumer<ConfirmCourier>
    {
        public async Task Consume(ConsumeContext<ConfirmCourier> context)
        {
            await context.Publish<CourierConfirmed>(new
            {
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.RestaurantId
            });
        }
    }
}