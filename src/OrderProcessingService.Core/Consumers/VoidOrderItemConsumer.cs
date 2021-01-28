namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class VoidOrderItemConsumer :
        IConsumer<VoidOrderItem>
    {
        readonly ILogger<VoidOrderItemConsumer> _logger;
        readonly IOrderProcessor _client;

        public VoidOrderItemConsumer(IGrpcClient<IOrderProcessor> grpcClient, ILogger<VoidOrderItemConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<VoidOrderItem> context)
        {
            _logger.LogInformation($"Consumer - {nameof(VoidOrderItemConsumer)} => consumed {nameof(VoidOrderItem)} event");

            var result = await _client.ChangeOrderItemStatus(
                new()
                {
                    OrderItemId = context.Message.OrderItemId,
                    Status = OrderItemStatus.Voided
                });

            var expectedResult = await _client.ChangeExpectedOrderItemStatus(
                new()
                {
                    OrderItemId = context.Message.OrderItemId,
                    Status = OrderItemStatus.Voided
                });

            if (result.IsSuccessful && expectedResult.IsSuccessful)
            {
                await context.Publish<OrderItemVoided>(
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