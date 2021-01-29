namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class PrepareOrderConsumer :
        IConsumer<PrepareOrder>
    {
        readonly ILogger<PrepareOrderConsumer> _logger;
        readonly IOrderProcessor _client;

        public PrepareOrderConsumer(IGrpcClient<IOrderProcessor> grpcClient, ILogger<PrepareOrderConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<PrepareOrder> context)
        {
            _logger.LogInformation($"Consumer - {nameof(PrepareOrderConsumer)} => consumed {nameof(PrepareOrder)} event");

            var result = await _client.AddNewOrder(
                new()
                {
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId,
                    RestaurantId = context.Message.RestaurantId,
                    AddressId = context.Message.AddressId
                });
            
            if (result.IsSuccessful)
            {
                for (int i = 0; i < context.Message.Items.Length; i++)
                {
                    await context.Publish<RequestOrderItemPreparation>(
                        new
                        {
                            context.Message.OrderId,
                            context.Message.RestaurantId,
                            context.Message.Items[i].OrderItemId,
                            context.Message.Items[i].MenuItemId,
                            context.Message.Items[i].SpecialInstructions
                        });

                    _logger.LogInformation($"Published - {nameof(RequestOrderItemPreparation)} (OrderItemId => {context.Message.Items[i].OrderItemId})");
                }
            }
        }
    }
}