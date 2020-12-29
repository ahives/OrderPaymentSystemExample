namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class CourierConfirmationConsumer :
        IConsumer<ConfirmCourier>
    {
        readonly ICourierDispatcher _dispatcher;

        public CourierConfirmationConsumer(ICourierDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Consume(ConsumeContext<ConfirmCourier> context)
        {
            var result = await _dispatcher.Confirm(context.Message.CourierId);
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierConfirmed>(new
                {
                    CourierId = result.Value,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
            }
        }
    }
}