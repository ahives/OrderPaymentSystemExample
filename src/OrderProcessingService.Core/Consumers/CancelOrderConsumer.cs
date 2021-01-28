namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class CancelOrderConsumer :
        IConsumer<CancelOrder>
    {
        readonly IOrderProcessor _client;

        public CancelOrderConsumer(IGrpcClient<IOrderProcessor> grpcClient)
        {
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<CancelOrder> context)
        {
            Log.Information($"Consumer - {nameof(CancelOrderConsumer)} => consumed {nameof(CancelOrder)} event");

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
                
                Log.Information($"Published - {nameof(OrderCanceled)}");
            }
        }
    }
}