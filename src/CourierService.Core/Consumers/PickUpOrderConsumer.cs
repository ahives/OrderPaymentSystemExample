namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class PickUpOrderConsumer :
        IConsumer<PickUpOrder>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<PickUpOrderConsumer> _logger;

        public PickUpOrderConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<PickUpOrderConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PickUpOrder> context)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(PickUpOrderConsumer)} => consumed {nameof(PickUpOrder)} event");

            var result = await _client.PickUpOrder(
                new()
                {
                    CourierId = context.Message.CourierId,
                    RestaurantId = context.Message.RestaurantId,
                    OrderId = context.Message.OrderId
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderPickedUp>(
                    new
                    {
                        context.Message.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderPickedUp)}");
            }
            else
            {
                if (result.Reason == ReasonType.RestaurantNotActive || result.Reason == ReasonType.RestaurantNotOpen)
                {
                    await context.Publish<CancelOrderRequest>(
                        new
                        {
                            context.Message.OrderId,
                            context.Message.CourierId,
                            context.Message.CustomerId,
                            context.Message.RestaurantId
                        });
                    
                    // TODO: courier status should be arrived at restaurant
                    
                    _logger.LogInformation($"Published - {nameof(CancelOrderRequest)}");
                }
            }
        }
    }
}