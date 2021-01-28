namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class VoidOrderItemConsumer :
        IConsumer<VoidOrderItem>
    {
        readonly IOrderProcessor _client;

        public VoidOrderItemConsumer(IGrpcClient<IOrderProcessor> grpcClient)
        {
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<VoidOrderItem> context)
        {
            Log.Information($"Consumer - {nameof(VoidOrderItemConsumer)} => consumed {nameof(VoidOrderItem)} event");

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
                
                Log.Information($"Published - {nameof(OrderItemCanceled)}");
            }
            else
            {
                
            }
        }
    }
}