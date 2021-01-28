namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchIdentificationConsumer :
        IConsumer<IdentifyCourierForDispatch>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<DispatchIdentificationConsumer> _logger;

        public DispatchIdentificationConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<DispatchIdentificationConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IdentifyCourierForDispatch> context)
        {
            _logger.LogInformation($"Consumer - {nameof(DispatchIdentificationConsumer)} => consumed {nameof(IdentifyCourierForDispatch)} event");

            var result = await _client.Identify(new () {CustomerId = context.Message.CustomerId});

            if (result.IsSuccessful)
            {
                await context.Publish<CourierIdentifiedForDispatch>(
                    new()
                    {
                        CourierId = result.Value.CourierId,
                        OrderId = context.Message.OrderId,
                        RestaurantId = context.Message.RestaurantId,
                        CustomerId = context.Message.CustomerId
                    });
                
                _logger.LogInformation($"Identified courier - {result.Value.CourierId}");
                _logger.LogInformation($"Published - {nameof(CourierIdentifiedForDispatch)}");
            }
            else
            {
                await context.Publish<CourierNotIdentifiedForDispatch>(
                    new()
                    {
                        OrderId = context.Message.OrderId,
                        RestaurantId = context.Message.RestaurantId,
                        CustomerId = context.Message.CustomerId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierNotIdentifiedForDispatch)}");
            }
        }
    }
}