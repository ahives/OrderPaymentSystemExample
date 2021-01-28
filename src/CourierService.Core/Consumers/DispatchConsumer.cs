namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchConsumer :
        IConsumer<DispatchCourier>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<DispatchConsumer> _logger;

        public DispatchConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<DispatchConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DispatchCourier> context)
        {
            _logger.LogInformation($"Consumer - {nameof(DispatchConsumer)} => consumed {nameof(DispatchCourier)} event");

            var result = await _client.ChangeStatus(
                new()
                {
                    CourierId = context.Message.CourierId,
                    Status = CourierStatus.Dispatched
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatched>(
                    new
                    {
                        result.Value.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierDispatched)}");
            }
            else
            {
                await context.Publish<CourierNotDispatched>(
                    new
                    {
                        result.Value.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierNotDispatched)}");
            }
        }
    }
}