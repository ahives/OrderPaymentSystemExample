namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class EnRouteToRestaurantConsumer :
        IConsumer<CourierEnRouteToRestaurant>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<EnRouteToRestaurantConsumer> _logger;

        public EnRouteToRestaurantConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<EnRouteToRestaurantConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CourierEnRouteToRestaurant> context)
        {
            _logger.LogInformation($"Consumer - {nameof(EnRouteToRestaurantConsumer)} => consumed {nameof(CourierEnRouteToRestaurant)} event");

            var result = await _client.ChangeStatus(
                new()
                {
                    CourierId = context.Message.CourierId,
                    Status = CourierStatus.EnRouteToRestaurant
                });
        }
    }
}