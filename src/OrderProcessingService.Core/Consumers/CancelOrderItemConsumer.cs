namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class CancelOrderItemConsumer :
        IConsumer<CancelOrderItem>
    {
        readonly ILogger<CancelOrderItemConsumer> _logger;
        readonly IOrderProcessor _client;

        public CancelOrderItemConsumer(IGrpcClient<IOrderProcessor> grpcClient, ILogger<CancelOrderItemConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<CancelOrderItem> context)
        {
            _logger.LogInformation($"Consumer - {nameof(CancelOrderItemConsumer)} => consumed {nameof(CancelOrderItem)} event");

            var result = await _client.ChangeOrderItemStatus(
                new()
                {
                    OrderItemId = context.Message.OrderItemId,
                    Status = OrderItemStatus.Canceled
                });

            var expectedResult = await _client.ChangeExpectedOrderItemStatus(
                new()
                {
                    OrderItemId = context.Message.OrderItemId,
                    Status = OrderItemStatus.Canceled
                });

            if (result.IsSuccessful && expectedResult.IsSuccessful)
            {
                await context.Publish<OrderItemCanceled>(
                    new()
                    {
                        OrderId = context.Message.OrderId,
                        OrderItemId = context.Message.OrderItemId,
                        CustomerId = context.Message.CustomerId,
                        RestaurantId = context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderItemCanceled)}");
            }
            else
            {
                
            }
        }
    }
}