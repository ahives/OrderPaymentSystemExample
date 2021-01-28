namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderDeliveryConsumer :
        IConsumer<DeliverOrder>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<OrderDeliveryConsumer> _logger;

        public OrderDeliveryConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<OrderDeliveryConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeliverOrder> context)
        {
            _logger.LogInformation($"Consumer - {nameof(OrderDeliveryConsumer)} => consumed {nameof(DeliverOrder)} event");

            var result = await _client.Deliver(
                new()
                {
                    CourierId = context.Message.CourierId
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderDelivered>(
                    new
                    {
                        context.Message.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(OrderDelivered)}");
            }
            else
            {
                var statusResult = await _client.ChangeStatus(
                    new()
                    {
                        CourierId = context.Message.CourierId,
                        Status = CourierStatus.ArrivedAtCustomer
                    });

                if (!statusResult.IsSuccessful)
                {
                    // TODO: fault the consumer and retry
                }
            }
        }
    }
}