namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class PrepareOrderItemConsumer :
        IConsumer<PrepareOrderItem>
    {
        readonly ILogger<PrepareOrderItemConsumer> _logger;
        readonly IOrderProcessor _client;

        public PrepareOrderItemConsumer(IGrpcClient<IOrderProcessor> grpcClient, ILogger<PrepareOrderItemConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<PrepareOrderItem> context)
        {
            _logger.LogInformation($"Consumer - {nameof(PrepareOrderItemConsumer)} => consumed {nameof(PrepareOrderItem)} event");

            var result = await _client.AddNewOrderItem(
                new()
                {
                    OrderId = context.Message.OrderId,
                    OrderItemId = context.Message.OrderItemId,
                    MenuItemId = context.Message.MenuItemId,
                    SpecialInstructions = context.Message.SpecialInstructions
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderItemPrepared>(
                    new
                    {
                        context.Message.OrderId,
                        result.Value.OrderItemId,
                        result.Value.Status,
                        result.Value.ShelfId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderItemPrepared)}");
            }
            else
            {
                await context.Publish<OrderItemNotPrepared>(
                    new
                    {
                        context.Message.OrderId,
                        result.Value.OrderItemId,
                        result.Value.Status,
                        result.Value.ShelfId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderItemNotPrepared)}");
            }
        }
    }
}