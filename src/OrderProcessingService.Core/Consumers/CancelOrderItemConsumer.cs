namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class CancelOrderItemConsumer :
        IConsumer<CancelOrderItem>
    {
        readonly IGrpcClient<IOrderProcessor> _client;

        public CancelOrderItemConsumer(IGrpcClient<IOrderProcessor> client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<CancelOrderItem> context)
        {
            Log.Information($"Consumer - {nameof(CancelOrderItemConsumer)} => consumed {nameof(CancelOrderItem)} event");
            
            var result = await _client.Client.ChangeOrderItemStatus(new CancelOrderItemContext()
            {
                OrderItemId = context.Message.OrderItemId,
                Status = OrderItemStatus.Canceled
            });

            if (result.IsSuccessful)
            {
                await context.Publish<OrderItemCanceled>(new ()
                {
                    OrderItemId = context.Message.OrderItemId
                });
            }
            else
            {
                
            }
        }
    }
}