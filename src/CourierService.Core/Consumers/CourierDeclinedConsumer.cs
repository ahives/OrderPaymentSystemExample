namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class CourierDeclinedConsumer :
        IConsumer<DeclineCourier>
    {
        readonly ICourierDispatcher _dispatcher;

        public CourierDeclinedConsumer(ICourierDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Consume(ConsumeContext<DeclineCourier> context)
        {
            var result = await _dispatcher.Decline(context.Message.CourierId);
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchDeclined>(new
                {
                    context.Message.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
            }
        }
    }
}