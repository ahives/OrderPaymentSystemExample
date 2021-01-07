namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
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
            Log.Information($"Courier State Machine - {nameof(PickUpOrderConsumer)}");
            
            var result = await _dispatcher.PickUpOrder(new ()
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
                
                Log.Information($"Courier {result.Value.CourierId} picked up order {result.Value.OrderId}");
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
                    
                    Log.Information($"Courier {result.Value.CourierId} Restaurant {result.Value.RestaurantId} was not opened.");
                }
            }
        }
    }
}