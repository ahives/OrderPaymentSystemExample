namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class CancelOrderConsumer :
        IConsumer<CancelOrder>
    {
        readonly ILogger<CancelOrderConsumer> _logger;
        readonly IOrderProcessor _client;

        public CancelOrderConsumer(IGrpcClient<IOrderProcessor> grpcClient, ILogger<CancelOrderConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<CancelOrder> context)
        {
            _logger.LogInformation($"Consumer - {nameof(CancelOrderConsumer)} => consumed {nameof(CancelOrder)} event");

            var result = await _client.ChangeOrderStatus(
                new()
                {
                    OrderId = context.Message.OrderId,
                    Status = OrderStatus.Canceled
                });

            if (result.IsSuccessful)
            {
                await context.Publish<OrderCanceled>(
                    new()
                    {
                        OrderId = context.Message.OrderId,
                        CourierId = context.Message.CourierId,
                        CustomerId = context.Message.CustomerId,
                        RestaurantId = context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderCanceled)}");
            }
        }
    }
}