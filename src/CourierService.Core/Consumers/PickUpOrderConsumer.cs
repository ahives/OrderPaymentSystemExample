namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class PickUpOrderConsumer :
        IConsumer<PickUpOrder>
    {
        readonly ICourierDispatcher _dispatcher;

        public PickUpOrderConsumer(ICourierDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Consume(ConsumeContext<PickUpOrder> context)
        {
            var result = await _dispatcher.PickUpOrder(new OrderPickUpCriteria()
            {
                CourierId = context.Message.CourierId,
                RestaurantId = context.Message.RestaurantId,
                OrderId = context.Message.OrderId
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderPickedUp>(new
                {
                    context.Message.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
            }
            else
            {
                if (result.Reason == ReasonType.RestaurantNotActive || result.Reason == ReasonType.RestaurantNotOpen)
                {
                    await context.Publish<CancelOrderRequest>(new
                    {
                        context.Message.OrderId,
                        context.Message.CourierId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                }
            }
        }
    }
}